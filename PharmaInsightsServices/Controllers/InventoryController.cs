using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaInsightsServices.DTOs;
using PharmaInsightsServices.Models;

namespace PharmaInsightsServices.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [Authorize]
    [HttpPost("import")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> ImportInventory([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var supportedTypes = new[] { ".xls", ".xlsx" };
        var fileExtension = Path.GetExtension(file.FileName).ToLower();
        if (!supportedTypes.Contains(fileExtension))
            return BadRequest("Invalid file type. Only Excel files are allowed.");

        try
        {
            var result = await _inventoryService.ImportInventoryAsync(file);

            if (result.Errors.Count > 0)
            {
                return BadRequest(new
                {
                    Message = "Some rows failed to import.",
                    Errors = result.Errors
                });
            }

            return Ok(new
            {
                Message = "Inventory imported successfully.",
                ImportedCount = result.ImportedCount
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var inventories = await _inventoryService.GetAllAsync();

        // Map entities to DTOs
        var inventoryDtos = inventories.Select(i => new InventoryDto
        {
            InventoryId = i.InventoryId,
            PharmacyId = i.PharmacyId,
            ProductId = i.ProductId,
            QuantityEntered = i.QuantityEntered,
            QuantitySold = i.QuantitySold,
            EntryDate = i.EntryDate,
            SaleDate = i.SaleDate
        });

        return Ok(inventoryDtos);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddInventory([FromBody] InventoryDto inventoryDto)
    {
        if (inventoryDto == null)
            return BadRequest("Inventory data cannot be null.");

        var inventory = new Inventory
        {
            PharmacyId = inventoryDto.PharmacyId,
            ProductId = inventoryDto.ProductId,
            QuantityEntered = inventoryDto.QuantityEntered,
            QuantitySold = inventoryDto.QuantitySold,
            EntryDate = inventoryDto.EntryDate,
            SaleDate = inventoryDto.SaleDate
        };

        await _inventoryService.AddInventoryAsync(inventory);
        return CreatedAtAction(nameof(GetAll), new { inventory_id = inventory.InventoryId }, inventory);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateInventory(int id, [FromBody] InventoryDto inventoryDto)
    {
        if (inventoryDto == null)
            return BadRequest("Inventory data cannot be null.");

        var updatedInventory = new Inventory
        {
            InventoryId = id,
            PharmacyId = inventoryDto.PharmacyId,
            ProductId = inventoryDto.ProductId,
            QuantityEntered = inventoryDto.QuantityEntered,
            QuantitySold = inventoryDto.QuantitySold,
            EntryDate = inventoryDto.EntryDate,
            SaleDate = inventoryDto.SaleDate
        };

        var result = await _inventoryService.UpdateInventoryAsync(id, updatedInventory);

        if (!result)
            return NotFound($"Inventory with ID {id} not found.");

        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInventory(int id)
    {
        var result = await _inventoryService.DeleteInventoryAsync(id);

        if (!result)
            return NotFound($"Inventory with ID {id} not found.");

        return NoContent();
    }

    [Authorize]
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummaryByPharmacyAndProduct()
    {
        var summaries = await _inventoryService.GetSummaryByPharmacyAndProductAsync();
        return Ok(summaries);
    }
}
