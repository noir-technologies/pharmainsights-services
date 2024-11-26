using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmaInsightsServices.Models;

public class User
{
    [Key]
    [Column("user_id")]
    public int UserId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("email")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [ForeignKey("Pharmacy")]
    [Column("pharmacy_id")]
    public int PharmacyId { get; set; }

    public Pharmacy? Pharmacy { get; set; }
}
