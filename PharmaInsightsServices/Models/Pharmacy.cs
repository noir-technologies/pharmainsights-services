using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmaInsightsServices.Models;

public class Pharmacy
{
    [Key]
    [Column("pharmacy_id")]
    public int PharmacyId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("name")]
    public required string Name { get; set; }

    [Required]
    [MaxLength(255)]
    [Column("location")]
    public required string Location { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
}
