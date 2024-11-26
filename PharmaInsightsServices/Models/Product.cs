using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmaInsightsServices.Models;

public class Product
{
    [Key]
    [Column("product_id")]
    public int ProductId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("name")]
    public required string Name { get; set; }

    [Required]
    [Column("description")]
    public required string Description { get; set; }

    [Required]
    [Column("price", TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }
}
