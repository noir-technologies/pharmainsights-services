using PharmaInsightsServices.Models;

public interface IPharmacyService
{
    Task<IEnumerable<Pharmacy>> GetAllAsync();
    Task AddPharmacyAsync(Pharmacy pharmacy);
    Task<bool> UpdatePharmacyAsync(int id, Pharmacy updatedPharmacy);
    Task<bool> DeletePharmacyAsync(int id);
}
