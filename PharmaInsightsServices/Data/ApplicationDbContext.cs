namespace PharmaInsightsServices.Data;

using Microsoft.EntityFrameworkCore;
using PharmaInsightsServices.Models;

public class ApplicationDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
}
