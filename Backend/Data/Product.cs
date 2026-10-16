using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Data;

[Table("products")]
public class Product
{
    [Column("id")]
    public int Id { get; set; }

    [Column("link")]
    public required string Link { get; set; }

    [Column("url")]
    public string? Url { get; set; }

    [Column("name")]
    public string? Name { get; set; }

    [Column("brand")]
    public string? Brand { get; set; }

    [Column("price_cents")]
    public int? PriceCents { get; set; }

    [Column("category")]
    public string? Category { get; set; }

    [Column("image")]
    public string? Image { get; set; }

    public ICollection<ProductWeight> Weights { get; set; } = new List<ProductWeight>();
}
