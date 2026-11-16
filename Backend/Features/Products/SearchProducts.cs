using Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend.Features.Products;

public static class SearchProducts
{
    public static async Task<IResult> Handle(string query, AppDbContext db)
    {
        var results = await (
                from product in db.Products
                where EF.Functions.ILike(product.Name, $"%{query}%")
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
                    product.Weights.Select(w => w.Weight).ToList()))
            .Take(20)
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
    List<string> Weights
);