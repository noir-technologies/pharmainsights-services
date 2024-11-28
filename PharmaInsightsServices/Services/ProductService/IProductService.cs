using PharmaInsightsServices.Models;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task AddProductAsync(Product product);
    Task<ImportResultDto> ImportProductsAsync(IFormFile file);
    Task<bool> UpdateProductAsync(int id, Product updatedProduct);
    Task<bool> DeleteProductAsync(int id);
}
