using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmaInsightsServices.Models;

public class Inventory
{
    [Key]
    [Column("inventory_id")]
    public int InventoryId { get; set; }

    [ForeignKey("Pharmacy")]
    [Column("pharmacy_id")]
    public int PharmacyId { get; set; }

    [ForeignKey("Product")]
    [Column("product_id")]
    public int ProductId { get; set; }

    [Required]
    [Column("quantity")]
    [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a non-negative value.")]
    public int Quantity { get; set; }

    public Pharmacy? Pharmacy { get; set; }
    public Product? Product { get; set; }
}
