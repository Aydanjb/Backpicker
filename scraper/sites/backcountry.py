import json

from common import NotFoundError, Site


def next_data(page):
    script = page.locator("script#__NEXT_DATA__")
    if script.count() == 0:
        return None
    return json.loads(script.text_content())["props"]["pageProps"]


class Backcountry(Site):
    name = "backcountry"
    base_url = "https://www.backcountry.com"
    total_pages = 203

    def listing_url(self, page_number):
        url = f"{self.base_url}/cat/hike-camp"
        return url if page_number == 1 else f"{url}?page={page_number}"

    def listing_links(self, page):
        props = next_data(page)
        if props is None:
            return None
        edges = props.get("plpData", {}).get("data", {}).get("category", {}).get("edges") or []
        return [edge["node"]["url"] for edge in edges] or None

    def product_data(self, page):
        props = next_data(page)
        if props is None:
            return None
        if not props.get("product"):
            raise NotFoundError(f"No product on {page.url}")
        return props["product"]

    def product_key(self, link):
        """Links are a single slug: /big-agnes-divide-insulated-sleeping-pad."""
        return link.strip("/")
