using System.ComponentModel.DataAnnotations;

namespace Backend.Data.Models;

public class Product
{
    public int Id { get; set; }

    [MaxLength(255)] public required string MatchKey { get; set; }

    [MaxLength(255)] public required string Name { get; set; }

    [MaxLength(255)] public required string Brand { get; set; }

    [MaxLength(255)] public required string Category { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<ProductWeight> Weights { get; } = [];
    public List<RetailerProduct> RetailerProducts { get; } = [];
}