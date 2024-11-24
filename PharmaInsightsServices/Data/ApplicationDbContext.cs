namespace PharmaInsightsServices.Data;

using Microsoft.EntityFrameworkCore;
using PharmaInsightsServices.Models;

public class ApplicationDbContext : DbContext
{
    public DbSet<Product> Product { get; set; }
    public DbSet<Pharmacy> Pharmacy { get; set; }
    public DbSet<User> User { get; set; }
    public DbSet<Inventory> Inventory { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
}
