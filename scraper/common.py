from abc import ABC, abstractmethod


class BlockedError(Exception):
    """The site refused us (403 or a challenge page). Site is skipped."""


class RateLimitedError(Exception):
    """The site asked us to slow down (429) or is struggling (5xx)."""

    def __init__(self, message, retry_after=None):
        super().__init__(message)
        self.retry_after = retry_after


class NotFoundError(Exception):
    """The page is gone (404 or 410), or redirected away."""


class Site(ABC):
    """
    Everything that differs between retailers.
    """

    name: str
    base_url: str
    total_pages: int

    @abstractmethod
    def listing_url(self, page_number):
        """The absolute URL of a listing page."""

    @abstractmethod
    def listing_links(self, page):
        """Product links on the listing page."""

    @abstractmethod
    def product_data(self, page):
        """
        The product JSON from the product page. Raises NotFoundError if the page isn't a product.
        """

    @abstractmethod
    def product_key(self, link):
        """The retailer's own ID for a product link."""
