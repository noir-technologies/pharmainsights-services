using PharmaInsightsServices.Data;
using PharmaInsightsServices.Models;
using Microsoft.EntityFrameworkCore;

public class InventoryService : IInventoryService
{
    private readonly ApplicationDbContext _context;

    public InventoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Inventory>> GetAllAsync()
    {
        return await _context.Inventory.ToListAsync();
    }

    public async Task AddInventoryAsync(Inventory inventory)
    {
        _context.Inventory.Add(inventory);
        await _context.SaveChangesAsync();
    }
    
    public async Task<bool> UpdateInventoryAsync(int id, Inventory updatedInventory)
    {
        var inventory = await _context.Inventory.FindAsync(id);

        if (inventory == null)
            return false;

        inventory.PharmacyId = updatedInventory.PharmacyId;
        inventory.ProductId = updatedInventory.ProductId;
        inventory.Quantity = updatedInventory.Quantity;

        _context.Inventory.Update(inventory);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteInventoryAsync(int id)
    {
        var inventory = await _context.Inventory.FindAsync(id);

        if (inventory == null)
            return false;

        _context.Inventory.Remove(inventory);
        await _context.SaveChangesAsync();

        return true;
    }

}
