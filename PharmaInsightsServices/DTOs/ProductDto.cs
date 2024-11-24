namespace PharmaInsightsServices.DTOs;

public class ProductDto
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public decimal Price { get; set; }
}
