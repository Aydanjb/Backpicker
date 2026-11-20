using System.ComponentModel.DataAnnotations;

namespace Data.Models;

public class RetailerProduct
{
    public int Id { get; set; }

    [MaxLength(255)] public string? Sku { get; set; }

    [MaxLength(255)] public required string ProductUrl { get; set; }

    [MaxLength(255)] public string? ProductImageUrl { get; set; }

    public int PriceCents { get; set; }

    public double? Rating { get; set; }
    public int ReviewCount { get; set; }

    public string? WeightRaw { get; set; }
    public string? CategoryRaw { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public int ProductId { get; set; }
    public required Product Product { get; set; }
    public int RetailerId { get; set; }
    public required Retailer Retailer { get; set; }
}