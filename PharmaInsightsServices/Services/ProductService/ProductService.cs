using PharmaInsightsServices.Data;
using PharmaInsightsServices.Models;
using OfficeOpenXml;
using Microsoft.EntityFrameworkCore;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;

    public ProductService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Product.ToListAsync();
    }

    public async Task AddProductAsync(Product product)
    {
        _context.Product.Add(product);
        await _context.SaveChangesAsync();
    }
    
    public async Task<ImportResultDto> ImportProductsAsync(IFormFile file)
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

                for (int row = 2; row <= rowCount; row++) // Start from row 2 (assuming headers in row 1)
                {
                    try
                    {
                        var name = worksheet.Cells[row, 1].Text.Trim();
                        var description = worksheet.Cells[row, 2].Text.Trim();
                        var price = decimal.Parse(worksheet.Cells[row, 3].Text.Trim());

                        var product = new Product
                        {
                            Name = name,
                            Description = description,
                            Price = price
                        };

                        await AddProductAsync(product);
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
    
    public async Task<bool> UpdateProductAsync(int id, Product updatedProduct)
    {
        var product = await _context.Product.FindAsync(id);

        if (product == null)
            return false;

        product.Name = updatedProduct.Name;
        product.Description = updatedProduct.Description;
        product.Price = updatedProduct.Price;

        _context.Product.Update(product);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _context.Product.FindAsync(id);

        if (product == null)
            return false;

        _context.Product.Remove(product);
        await _context.SaveChangesAsync();

        return true;
    }
}
