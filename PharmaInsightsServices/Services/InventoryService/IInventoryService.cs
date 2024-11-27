using PharmaInsightsServices.Models;

public interface IInventoryService
{
    Task<IEnumerable<Inventory>> GetAllAsync();
    Task AddInventoryAsync(Inventory inventory);
    Task<ImportResultDto> ImportInventoryAsync(IFormFile file);
    Task<bool> UpdateInventoryAsync(int id, Inventory updatedInventory);
    Task<bool> DeleteInventoryAsync(int id);
}
