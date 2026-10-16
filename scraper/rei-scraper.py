import datetime
import json
import logging
import sqlite3

from camoufox.sync_api import Camoufox

BASE_URL = "https://www.rei.com"
START_URL = f"{BASE_URL}/c/camping-and-hiking"
WEIGHT_LABELS = ["Weight", "Packaged Weight"]
MAX_PRODUCTS_PER_RUN = 100
TOTAL_PAGES = (
    146  # Known page count for camping-and-hiking; avoids an extra visit to check
)
DB_FILE = "products.db"

logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s - [%(levelname)s] - %(message)s",
    handlers=[
        logging.FileHandler(
            f"./logs/{datetime.datetime.now().strftime('%Y-%m-%d_%H-%M-%S')}.log"
        ),
        logging.StreamHandler(),
    ],
)

log = logging.getLogger("rei-scraper")


def init_db(conn):
    conn.execute("""
        CREATE TABLE IF NOT EXISTS products (
            id INTEGER PRIMARY KEY,
            link TEXT UNIQUE,
            url TEXT,
            name TEXT,
            price_cents INTEGER,
            category TEXT,
            image TEXT
        )
    """)
    conn.execute("""
        CREATE TABLE IF NOT EXISTS product_weights (
            product_id INTEGER REFERENCES products(id),
            weight TEXT
        )
    """)
    conn.commit()

    # products.db may already exist from before the `brand` column was added;
    # CREATE TABLE IF NOT EXISTS won't add it to an existing table.
    columns = {row[1] for row in conn.execute("PRAGMA table_info(products)")}
    if "brand" not in columns:
        log.info("Migrating products table: adding 'brand' column.")
        conn.execute("ALTER TABLE products ADD COLUMN brand TEXT")
        conn.commit()


def save_product(conn, link, product):
    conn.execute(
        """
        INSERT INTO products (link, url, name, brand, price_cents, category, image)
        VALUES (:link, :url, :name, :brand, :price_cents, :category, :image)
        ON CONFLICT(link) DO UPDATE SET
            url=excluded.url, name=excluded.name, brand=excluded.brand,
            price_cents=excluded.price_cents, category=excluded.category,
            image=excluded.image
        """,
        {
            "link": link,
            "url": product["url"],
            "name": product["name"],
            "brand": product["brand"],
            "price_cents": product["price_cents"],
            "category": product["category"],
            "image": product["image"],
        },
    )
    product_id = conn.execute(
        "SELECT id FROM products WHERE link = ?", (link,)
    ).fetchone()[0]

    conn.execute("DELETE FROM product_weights WHERE product_id = ?", (product_id,))
    if product["weight"]:
        conn.executemany(
            "INSERT INTO product_weights (product_id, weight) VALUES (?, ?)",
            [(product_id, w) for w in product["weight"]],
        )
    conn.commit()


conn = sqlite3.connect(DB_FILE)
init_db(conn)

scraped_links = {row[0] for row in conn.execute("SELECT link FROM products")}
log.info(f"Loaded {len(scraped_links)} already-scraped products from {DB_FILE}.")

with Camoufox(os="windows", headless=True, humanize=True) as browser:
    log.info("Starting REI scraper...")
    page = browser.new_page()

    scraped_count = 0

    for page_number in range(1, TOTAL_PAGES + 1):
        if scraped_count >= MAX_PRODUCTS_PER_RUN:
            log.info(
                f"Reached the {MAX_PRODUCTS_PER_RUN}-product limit for this run. "
                "Stopping early; re-run later to continue where this left off."
            )
            break

        log.info(f"Scraping page {page_number}...")
        page.goto(f"{START_URL}?page={page_number}")
        elements = page.locator("li[data-cnstrc-item-id]")
        links = [
            element.locator("a").first.get_attribute("href")
            for element in elements.all()
        ]

        for link in links:  # Scrape this page's products before moving to the next page
            if link in scraped_links:
                log.info(f"Product already scraped: {link}. Skipping...")
                continue

            if scraped_count >= MAX_PRODUCTS_PER_RUN:
                log.info(
                    f"Reached the {MAX_PRODUCTS_PER_RUN}-product limit for this run. "
                    "Stopping early; re-run later to continue where this left off."
                )
                break

            log.info(f"Navigating to product page: {link}")
            page.goto(BASE_URL + link)

            product_url = page.url.strip()
            model_data = json.loads(page.locator("script#modelData").text_content())
            product_data = model_data["pageData"]["product"]

            product_name = product_data["title"].strip()
            product_brand = product_data.get("brand", {}).get("name")
            product_category = product_data.get("primaryCategory", {}).get("name")

            images = product_data.get("images") or []
            product_image = (
                BASE_URL + images[0]["heroImageUrl"] if images else None
            )

            min_price = product_data.get("stylePrice", {}).get("minPrice", {}).get("value")
            product_price_cents = round(min_price * 100) if min_price is not None else None

            product_weight = None
            for spec in product_data.get("techSpecs", []):
                if spec["name"] in WEIGHT_LABELS:
                    product_weight = spec["values"]
                    break

            if product_weight is None:
                log.warning(
                    f"Weight not found for product: {product_name} ({product_url})"
                )

            product = {
                "url": product_url,
                "name": product_name,
                "brand": product_brand,
                "category": product_category,
                "price_cents": product_price_cents,
                "weight": product_weight,
                "image": product_image,
            }

            log.info(f"Saving {product['name']} to {DB_FILE}...")
            save_product(conn, link, product)
            scraped_links.add(link)
            scraped_count += 1

            log.info("Going back to the product list page...")
            page.go_back()

    log.info("Scraping completed. Closing the browser...")
    browser.close()

conn.close()
