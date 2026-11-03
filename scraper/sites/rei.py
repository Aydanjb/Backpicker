import json

from playwright.sync_api import TimeoutError as PlaywrightTimeoutError

from common import Site

LISTING_TILE_TIMEOUT_MS = 15_000


class Rei(Site):
    name = "rei"
    base_url = "https://www.rei.com"
    total_pages = 146

    def listing_url(self, page_number):
        return f"{self.base_url}/c/camping-and-hiking?page={page_number}"

    def listing_links(self, page):
        tiles = page.locator("li[data-cnstrc-item-id]")
        try:
            tiles.first.wait_for(state="attached", timeout=LISTING_TILE_TIMEOUT_MS)
        except PlaywrightTimeoutError:
            return None
        return [tile.locator("a").first.get_attribute("href") for tile in tiles.all()]

    def product_data(self, page):
        script = page.locator("script#modelData")
        if script.count() == 0:
            return None
        return json.loads(script.text_content())["pageData"]["product"]

    def product_key(self, link):
        """Links look like /product/<id>/<slug>."""
        return link.split("/")[2]
