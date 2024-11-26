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

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var inventories = await _inventoryService.GetAllAsync();
        
        // Map entities to DTOs
        var inventoryDtos = inventories.Select(i => new InventoryDto
        {
            PharmacyId = i.PharmacyId,
            ProductId = i.ProductId,
            Quantity = i.Quantity
        });

        return Ok(inventoryDtos);
    }

    [HttpPost]
    public async Task<IActionResult> AddInventory([FromBody] InventoryDto inventoryDto)
    {
        if (inventoryDto == null)
            return BadRequest("Inventory data cannot be null.");

        var inventory = new Inventory
        {
            PharmacyId = inventoryDto.PharmacyId,
            ProductId = inventoryDto.ProductId,
            Quantity = inventoryDto.Quantity
        };

        await _inventoryService.AddInventoryAsync(inventory);
        return CreatedAtAction(nameof(GetAll), new { inventory_id = inventory.InventoryId }, inventory);
    }
    
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
            Quantity = inventoryDto.Quantity
        };

        var result = await _inventoryService.UpdateInventoryAsync(id, updatedInventory);

        if (!result)
            return NotFound($"Inventory with ID {id} not found.");

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInventory(int id)
    {
        var result = await _inventoryService.DeleteInventoryAsync(id);

        if (!result)
            return NotFound($"Inventory with ID {id} not found.");

        return NoContent();
    }
}
