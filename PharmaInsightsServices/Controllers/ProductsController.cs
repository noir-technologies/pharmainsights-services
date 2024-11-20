namespace PharmaInsightsServices.Controllers;

using Microsoft.AspNetCore.Mvc;
using PharmaInsightsServices.Services;
using PharmaInsightsServices.Models;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }

    [HttpPost]
    public async Task<IActionResult> AddProduct([FromBody] Product product)
    {
        if (product == null)
            return BadRequest("Product cannot be null.");

        await _productService.AddProductAsync(product);
        return CreatedAtAction(nameof(GetAll), new { id = product.Id }, product);
    }
}
