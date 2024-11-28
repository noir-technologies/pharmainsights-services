using PharmaInsightsServices.Data;
using PharmaInsightsServices.Models;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using PharmaInsightsServices.DTOs;

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
        if (!await ForeignKeysExist(inventory.PharmacyId, inventory.ProductId))
            throw new Exception("Invalid PharmacyId or ProductId.");

        var existingInventory = await _context.Inventory
            .FirstOrDefaultAsync(i => i.PharmacyId == inventory.PharmacyId && i.ProductId == inventory.ProductId);

        if (existingInventory != null)
        {
            existingInventory.QuantityEntered += inventory.QuantityEntered;
            _context.Inventory.Update(existingInventory);
        }
        else
        {
            _context.Inventory.Add(new Inventory
            {
                PharmacyId = inventory.PharmacyId,
                ProductId = inventory.ProductId,
                QuantityEntered = inventory.QuantityEntered,
                QuantitySold = inventory.QuantitySold,
                EntryDate = inventory.EntryDate,
                SaleDate = inventory.SaleDate
            });
        }

        await _context.SaveChangesAsync();
    }

    public async Task<ImportResultDto> ImportInventoryAsync(IFormFile file)
    {
        var errors = new List<string>();
        var importedCount = 0;

        using (var stream = new MemoryStream())
        {
            await file.CopyToAsync(stream);
            stream.Position = 0;

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        var pharmacyId = int.Parse(worksheet.Cells[row, 1].Text.Trim());
                        var productId = int.Parse(worksheet.Cells[row, 2].Text.Trim());
                        var quantityEntered = int.Parse(worksheet.Cells[row, 3].Text.Trim());
                        var quantitySold = int.Parse(worksheet.Cells[row, 4].Text.Trim());
                        var entryDate = DateTime.Parse(worksheet.Cells[row, 5].Text.Trim());
                        var saleDate = DateTime.Parse(worksheet.Cells[row, 6].Text.Trim());

                        var inventory = new Inventory
                        {
                            PharmacyId = pharmacyId,
                            ProductId = productId,
                            QuantityEntered = quantityEntered,
                            QuantitySold = quantitySold,
                            EntryDate = entryDate,
                            SaleDate = saleDate
                        };

                        await AddInventoryAsync(inventory);
                        importedCount++;
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Row {row}: {ex.Message}");
                    }
                }
            }
        }

        return new ImportResultDto
        {
            ImportedCount = importedCount,
            Errors = errors
        };
    }

    public async Task<bool> UpdateInventoryAsync(int id, Inventory updatedInventory)
    {
        var inventory = await _context.Inventory.FindAsync(id);

        if (inventory == null)
            return false;

        inventory.PharmacyId = updatedInventory.PharmacyId;
        inventory.ProductId = updatedInventory.ProductId;
        inventory.QuantityEntered = updatedInventory.QuantityEntered;
        inventory.QuantitySold = updatedInventory.QuantitySold;
        inventory.EntryDate = updatedInventory.EntryDate;
        inventory.SaleDate = updatedInventory.SaleDate;

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

    private async Task<bool> ForeignKeysExist(int pharmacyId, int productId)
    {
        var pharmacyExists = await _context.Pharmacy.AnyAsync(p => p.PharmacyId == pharmacyId);
        var productExists = await _context.Product.AnyAsync(p => p.ProductId == productId);

        return pharmacyExists && productExists;
    }

    public async Task<IEnumerable<ProductInventorySummaryDto>> GetSummaryByPharmacyAndProductAsync()
    {
        return await _context.Inventory
            .Include(i => i.Pharmacy)
            .Include(i => i.Product)
            .GroupBy(i => new { i.Pharmacy!.PharmacyId, PharmacyName = i.Pharmacy.Name, i.Product!.ProductId, ProductName = i.Product.Name, i.Product.Price })
            .Select(g => new ProductInventorySummaryDto
            {
                PharmacyId = g.Key.PharmacyId,
                PharmacyName = g.Key.PharmacyName,
                ProductId = g.Key.ProductId,
                ProductName = g.Key.ProductName,
                Price = g.Key.Price,
                TotalUnitsEntered = g.Sum(i => i.QuantityEntered),
                TotalUnitsSold = g.Sum(i => i.QuantitySold),
                RemainingUnits = g.Sum(i => i.QuantityEntered) - g.Sum(i => i.QuantitySold)
            })
            .ToListAsync();
    }
}
