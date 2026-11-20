using System.ComponentModel.DataAnnotations;

namespace Data.Models;

public class Retailer
{
    public int Id { get; set; }

    [MaxLength(255)] public required string Name { get; set; }

    [MaxLength(255)] public string? Url { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<RetailerProduct> RetailerProducts { get; } = [];
}