using System.ComponentModel.DataAnnotations;

namespace Backend.Data.Models;

public class ProductWeight
{
    public int ProductId { get; set; }

    [MaxLength(255)] public required string Size { get; set; }

    [MaxLength(255)] public required string Weight { get; set; }

    public Product? Product { get; set; }
}