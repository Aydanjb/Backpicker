using Backend.Data;
using Microsoft.EntityFrameworkCore;
namespace Backend.Features.Products;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products");

        group.MapGet("/search", async (string query, AppDbContext db) =>
        {
            var results = await db.Products
                .Where(p => EF.Functions.Like(p.Name, $"%{query}%"))
                .OrderBy(p => p.Name)
                .Take(20)
                .Select(p => new ProductSearchResultDto(
                    p.Id,
                    p.Name,
                    p.Brand,
                    p.PriceCents,
                    p.Image,
                    p.Weights.Select(w => w.Weight).ToList()))
                .ToListAsync();

            return Results.Ok(results);
        });
    }
}