using System.ComponentModel.DataAnnotations;

namespace Data.Models;

public class ProductWeight
{
    public int ProductId { get; set; }

    [MaxLength(255)] public required string Size { get; set; }

    public required string Weight { get; set; }

    public int Grams { get; set; }

    public Product? Product { get; set; }
}