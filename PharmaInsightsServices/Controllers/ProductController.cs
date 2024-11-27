using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaInsightsServices.DTOs;
using PharmaInsightsServices.Models;

namespace PharmaInsightsServices.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productService.GetAllAsync();
        
        // Map entities to DTOs
        var productDtos = products.Select(p => new ProductDto
        {
            Name = p.Name,
            Description = p.Description,
            Price = p.Price
        });

        return Ok(productDtos);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddProduct([FromBody] ProductDto productDto)
    {
        if (productDto == null)
            return BadRequest("Product data cannot be null.");

        var product = new Product
        {
            Name = productDto.Name,
            Description = productDto.Description,
            Price = productDto.Price
        };

        await _productService.AddProductAsync(product);
        return CreatedAtAction(nameof(GetAll), new { product_id = product.ProductId }, product);
    }
    
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDto productDto)
    {
        if (productDto == null)
            return BadRequest("Product data cannot be null.");

        var updatedProduct = new Product
        {
            ProductId = id,
            Name = productDto.Name,
            Description = productDto.Description,
            Price = productDto.Price
        };

        var result = await _productService.UpdateProductAsync(id, updatedProduct);

        if (!result)
            return NotFound($"Product with ID {id} not found.");

        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var result = await _productService.DeleteProductAsync(id);

        if (!result)
            return NotFound($"Product with ID {id} not found.");

        return NoContent();
    }
}
