using PharmaInsightsServices.Data;
using PharmaInsightsServices.Models;
using Microsoft.EntityFrameworkCore;

public class PharmacyService : IPharmacyService
{
    private readonly ApplicationDbContext _context;

    public PharmacyService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Pharmacy>> GetAllAsync()
    {
        return await _context.Pharmacy.ToListAsync();
    }

    public async Task AddPharmacyAsync(Pharmacy pharmacy)
    {
        _context.Pharmacy.Add(pharmacy);
        await _context.SaveChangesAsync();
    }
    
    public async Task<bool> UpdatePharmacyAsync(int id, Pharmacy updatedPharmacy)
    {
        var pharmacy = await _context.Pharmacy.FindAsync(id);

        if (pharmacy == null)
            return false;

        pharmacy.Name = updatedPharmacy.Name;
        pharmacy.Location = updatedPharmacy.Location;

        _context.Pharmacy.Update(pharmacy);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeletePharmacyAsync(int id)
    {
        var pharmacy = await _context.Pharmacy.FindAsync(id);

        if (pharmacy == null)
            return false;

        _context.Pharmacy.Remove(pharmacy);
        await _context.SaveChangesAsync();

        return true;
    }
}
