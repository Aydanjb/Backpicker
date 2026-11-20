using Data;
using Microsoft.EntityFrameworkCore;

namespace Backend.Features.Products;

public static class SearchProducts
{
    public static async Task<IResult> Handle(string? query, string? slot, AppDbContext db)
    {
        var products = db.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
            products = products.Where(p => EF.Functions.ILike(p.Name, $"%{query}%"));

        if (slot is not null)
        {
            if (KitSlots.CategoriesFor(slot) is not { } categories)
                return Results.NotFound($"Unknown kit slot '{slot}'.");

            products = products.Where(p => p.RetailerProducts.Any(rp => categories.Contains(rp.CategoryRaw!)));
        }

        var results = await (
                from product in products
                orderby product.Name
                let cheapest = product.RetailerProducts
                    .OrderBy(rp => rp.PriceCents)
                    .FirstOrDefault()
                select new ProductSearchResultDto(
                    product.Id,
                    product.Name,
                    product.Brand,
                    cheapest.PriceCents,
                    cheapest.ProductUrl,
                    cheapest.ProductImageUrl,
                    cheapest.Retailer.Name,
                    cheapest.Rating,
                    cheapest.ReviewCount,
                    product.Weights
                        .OrderBy(w => w.Grams)
                        .Select(w => new WeightDto(w.Size, w.Grams))
                        .ToList()))
            .Take(50)
            .ToListAsync();

        return Results.Ok(results);
    }
}

public record ProductSearchResultDto(
    int Id,
    string Name,
    string Brand,
    int PriceCents,
    string Link,
    string? Image,
    string Retailer,
    double? Rating,
    int ReviewCount,
    List<WeightDto> Weights
);

public record WeightDto(string Size, int Grams);
