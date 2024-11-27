namespace PharmaInsightsServices.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class User
{
    [Key]
    [Column("user_id")]
    public int UserId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("name")]
    public required string Name { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("email")]
    public required string Email { get; set; }

    [Column("password_hash")]
    public required byte[] PasswordHash { get; set; }

    [Column("password_salt")]
    public required byte[] PasswordSalt { get; set; }

    [ForeignKey("Pharmacy")]
    [Column("pharmacy_id")]
    public int PharmacyId { get; set; }

    public Pharmacy? Pharmacy { get; set; }
}
