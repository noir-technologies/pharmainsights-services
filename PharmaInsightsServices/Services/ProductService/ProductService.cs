using PharmaInsightsServices.Data;
using PharmaInsightsServices.Models;
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
