using Data;
using Data.Models;
using Ingest.Normalization;
using Microsoft.EntityFrameworkCore;

namespace Ingest;

public record LoadResult(int Created, int Updated);

public class ListingLoader(AppDbContext db)
{
    public LoadResult Load(IEnumerable<(Snapshot Snapshot, NormalizedListing Normalized)> items)
    {
        var retailers = db.Retailers.ToDictionary(r => r.Name);
        var products = db.Products
            .Include(p => p.Weights)
            .ToDictionary(p => p.MatchKey);
        var listings = db.RetailerProducts
            .Include(rp => rp.Retailer)
            .ToDictionary(rp => (rp.Retailer.Name, rp.ProductUrl));

        var now = DateTime.UtcNow;
        var created = 0;
        var updated = 0;

        foreach (var (snapshot, normalized) in items)
        {
            var parsed = normalized.Listing;

            if (!retailers.TryGetValue(snapshot.Retailer, out var retailer))
            {
                retailer = new Retailer
                {
                    Name = snapshot.Retailer,
                    Url = new Uri(snapshot.Url).GetLeftPart(UriPartial.Authority),
                    CreatedAt = now,
                };
                db.Retailers.Add(retailer);
                retailers.Add(retailer.Name, retailer);
            }

            if (!products.TryGetValue(normalized.MatchKey, out var product))
            {
                product = new Product
                {
                    MatchKey = normalized.MatchKey,
                    Name = parsed.Name,
                    Brand = parsed.Brand,
                    Category = parsed.CategoryRaw,
                    CreatedAt = now,
                };
                db.Products.Add(product);
                products.Add(product.MatchKey, product);
            }

            if (product.Weights.Count == 0)
            {
                foreach (var weight in normalized.Weights)
                {
                    product.Weights.Add(new ProductWeight
                    {
                        Size = weight.Size,
                        Weight = weight.Raw,
                        Grams = weight.Grams,
                    });
                }
            }

            if (listings.TryGetValue((retailer.Name, snapshot.Url), out var listing))
            {
                updated++;
            }
            else
            {
                listing = new RetailerProduct
                {
                    ProductUrl = snapshot.Url,
                    Product = product,
                    Retailer = retailer,
                    CreatedAt = now,
                };
                db.RetailerProducts.Add(listing);
                listings.Add((retailer.Name, snapshot.Url), listing);
                created++;
            }

            listing.Product = product;
            listing.Sku = parsed.Sku;
            listing.ProductImageUrl = parsed.ImageUrl;
            listing.PriceCents = parsed.PriceCents;
            listing.Rating = parsed.Rating;
            listing.ReviewCount = parsed.ReviewCount;
            listing.WeightRaw = parsed.WeightRaw;
            listing.CategoryRaw = parsed.CategoryRaw;
            listing.UpdatedAt = now;
        }

        db.SaveChanges();

        return new LoadResult(created, updated);
    }
}
