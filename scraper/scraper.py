import datetime
import email.utils
import logging
import pickle
import random
import time
from itertools import islice
from pathlib import Path

from camoufox.fingerprints import generate_fingerprint
from camoufox.sync_api import Camoufox

import store
from common import BlockedError, NotFoundError, RateLimitedError
from sites import SITES

MAX_PRODUCTS_PER_RUN = 500
REFRESH_AFTER_DAYS = 7
REQUEST_DELAY_SECONDS = (8, 20)
RATE_LIMIT_RETRY_DELAYS_SECONDS = [120, 600, 1800]

SCRAPER_DIR = Path(__file__).resolve().parent
LOG_DIR = SCRAPER_DIR / "logs"
RAW_DB = SCRAPER_DIR / "raw.db"
PROFILE_DIR = SCRAPER_DIR / "browser-profile"
IDENTITY_FILE = SCRAPER_DIR / "identity.pkl"

log = logging.getLogger("backpicker-scraper")


def setup_logging():
    LOG_DIR.mkdir(exist_ok=True)
    logging.basicConfig(
        level=logging.INFO,
        format="%(asctime)s - [%(levelname)s] - %(message)s",
        handlers=[
            logging.FileHandler(
                LOG_DIR / f"{datetime.datetime.now():%Y-%m-%d_%H-%M-%S}.log"
            ),
            logging.StreamHandler(),
        ],
    )


def retry_on_rate_limit(action, description):
    """Runs action, backing off and retrying on RateLimitedError until the schedule runs out."""
    for attempt, delay in enumerate(RATE_LIMIT_RETRY_DELAYS_SECONDS, start=1):
        try:
            return action()
        except RateLimitedError as e:
            if (e.retry_after or 0) > RATE_LIMIT_RETRY_DELAYS_SECONDS[-1]:
                raise
            delay = max(delay, e.retry_after or 0)
            log.warning(
                f"Rate limited while {description} ({e}). Retry {attempt}/"
                f"{len(RATE_LIMIT_RETRY_DELAYS_SECONDS)} in {delay / 60:.0f} min..."
            )
            time.sleep(delay)
    return action()


def parse_retry_after(value):
    """Returns a Retry-After header in seconds; it's either a number of seconds or an HTTP date."""
    if not value:
        return None
    if value.isdigit():
        return int(value)
    try:
        retry_at = email.utils.parsedate_to_datetime(value)
    except (TypeError, ValueError):
        return None
    if retry_at.tzinfo is None:
        retry_at = retry_at.replace(tzinfo=datetime.timezone.utc)
    return max(0, (retry_at - datetime.datetime.now(datetime.timezone.utc)).total_seconds())


def load_identity():
    if IDENTITY_FILE.exists():
        return pickle.loads(IDENTITY_FILE.read_bytes())

    log.info(f"No saved identity; generating a new fingerprint in {IDENTITY_FILE.name}.")
    identity = {
        "fingerprint": generate_fingerprint(os="windows"),
        "config": {"fonts:spacing_seed": random.randint(0, 1_073_741_823)},
    }
    IDENTITY_FILE.write_bytes(pickle.dumps(identity))
    return identity


def open_browser():
    identity = load_identity()
    return Camoufox(
        fingerprint=identity["fingerprint"],
        config=identity["config"],
        i_know_what_im_doing=True,
        persistent_context=True,
        user_data_dir=PROFILE_DIR,
        enable_cache=True,
        block_images=True,
        geoip=True,
        humanize=True,
    )


def load(page, url, wait_until="load"):
    """Waits a random interval, loads the page and returns the response."""
    time.sleep(random.uniform(*REQUEST_DELAY_SECONDS))
    response = page.goto(url, wait_until=wait_until)
    if response is None:
        raise BlockedError(f"No response for {url}")

    status = response.status
    if status == 429 or status >= 500:
        raise RateLimitedError(
            f"HTTP {status} for {url}",
            retry_after=parse_retry_after(response.headers.get("retry-after")),
        )
    if status in (404, 410):
        raise NotFoundError(f"HTTP {status} for {url}")
    if status >= 400:
        raise BlockedError(f"HTTP {status} for {url}")
    return response


def load_listing_links(page, site, page_number):
    load(page, site.listing_url(page_number), wait_until="domcontentloaded")
    links = site.listing_links(page)
    if links is None:
        raise BlockedError(f"No products on {site.name} listing page {page_number}")
    return links


def new_product_links(page, conn, site, fresh_keys):
    """
    Yields links to products without a fresh snapshot. 
    Starts on the page the last run stopped on and wraps around once.
    """
    start = store.load_next_page(conn, site.name)
    if not 1 <= start <= site.total_pages:
        start = 1
    for page_number in [*range(start, site.total_pages + 1), *range(1, start)]:
        log.info(f"Scraping {site.name} page {page_number}...")
        try:
            links = retry_on_rate_limit(
                lambda: load_listing_links(page, site, page_number),
                f"loading {site.name} listing page {page_number}",
            )
        except NotFoundError:
            log.warning(
                f"{site.name} listing page {page_number} not found."
            )
            store.save_next_page(conn, site.name, 1)
            return

        for link in links:
            if site.product_key(link) in fresh_keys:
                continue
            yield link
        store.save_next_page(conn, site.name, page_number % site.total_pages + 1)


def fetch_product(page, conn, site, link):
    """Loads a product page and appends its JSON to the store as a new snapshot."""
    url = site.base_url + link
    log.info(f"Fetching {url}")
    load(page, url, wait_until="domcontentloaded")
    data = site.product_data(page)
    if data is None:
        raise BlockedError(f"No product data on {page.url}")
    store.save_snapshot(conn, site.name, site.product_key(link), url, data)


def scrape_site(page, conn, site):
    """
    Fetches up to MAX_PRODUCTS_PER_RUN products from one site that has no fresh snapshot, 
    and returns how many were fetched.
    """
    scraped_count = 0
    try:
        fresh_keys = store.fresh_keys(conn, site.name, REFRESH_AFTER_DAYS)
        links = islice(new_product_links(page, conn, site, fresh_keys), MAX_PRODUCTS_PER_RUN)
        for link in links:
            try:
                retry_on_rate_limit(
                    lambda: fetch_product(page, conn, site, link), f"loading {link}"
                )
            except NotFoundError as e:
                log.warning(f"Skipping {link}: {e}")
                continue
            fresh_keys.add(site.product_key(link))
            scraped_count += 1
    except (BlockedError, RateLimitedError) as e:
        log.error(f"Stopped by {site.name} ({e}).")
    return scraped_count


def main():
    setup_logging()
    conn = store.connect(RAW_DB)
    try:
        with open_browser() as context:
            log.info("Starting Backpicker scraper...")
            page = context.pages[0] if context.pages else context.new_page()
            for site in SITES:
                try:
                    scraped_count = scrape_site(page, conn, site)
                except Exception:
                    log.exception(f"{site.name} crashed; moving on to the next site.")
                    continue
                log.info(f"Fetched {scraped_count} {site.name} products this run.")
            log.info("Scraping completed. Closing the browser...")
    finally:
        conn.close()


if __name__ == "__main__":
    main()
