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
    [Column("quantity_entered")]
    [Range(0, int.MaxValue, ErrorMessage = "Quantity entered must be a non-negative value.")]
    public int QuantityEntered { get; set; }

    [Required]
    [Column("quantity_sold")]
    [Range(0, int.MaxValue, ErrorMessage = "Quantity sold must be a non-negative value.")]
    public int QuantitySold { get; set; }

    [Required]
    [Column("entry_date")]
    public DateTime EntryDate { get; set; }

    [Required]
    [Column("sale_date")]
    public DateTime SaleDate { get; set; }

    public Pharmacy? Pharmacy { get; set; }
    public Product? Product { get; set; }
}
