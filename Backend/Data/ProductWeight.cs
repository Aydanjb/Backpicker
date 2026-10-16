using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Data;

[Table("product_weights")]
public class ProductWeight
{
    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("weight")]
    public required string Weight { get; set; }

    public Product? Product { get; set; }
}
