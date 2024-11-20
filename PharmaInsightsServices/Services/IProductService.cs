using PharmaInsightsServices.Models;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task AddProductAsync(Product product);
}
