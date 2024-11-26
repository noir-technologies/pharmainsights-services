using Microsoft.AspNetCore.Mvc;
using PharmaInsightsServices.DTOs;
using PharmaInsightsServices.Models;
using OfficeOpenXml;

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

    // Endpoint para subir y leer datos de un archivo Excel
    [HttpPost("read-excel")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> ReadExcel([FromForm] IFormFile file)
    {
        
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var supportedTypes = new[] { ".xls", ".xlsx" };
        var fileExtension = Path.GetExtension(file.FileName).ToLower();
        if (!supportedTypes.Contains(fileExtension))
            return BadRequest("Invalid file type. Only Excel files are allowed.");

        var inventoryList = new List<InventoryDto>();

        using (var stream = new MemoryStream())
        {
            await file.CopyToAsync(stream);
            stream.Position = 0;

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets[0]; // Leer la primera hoja
                var rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++) // Asumiendo que la fila 1 tiene encabezados
                {
                    Console.WriteLine($"Reading row {row}...");
                    var pharmacyId = int.Parse(worksheet.Cells[row, 1].Text.Trim());
                    var productId = int.Parse(worksheet.Cells[row, 2].Text.Trim());
                    var quantity = int.Parse(worksheet.Cells[row, 3].Text.Trim());

                    inventoryList.Add(new InventoryDto
                    {
                        PharmacyId = pharmacyId,
                        ProductId = productId,
                        Quantity = quantity
                    });
                }

            }
        }

        return Ok(inventoryList); // Retorna los datos leídos como JSON
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
