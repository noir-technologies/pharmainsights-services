using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaInsightsServices.DTOs;
using PharmaInsightsServices.Models;

namespace PharmaInsightsServices.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PharmacyController : ControllerBase
{
    private readonly IPharmacyService _pharmacyService;

    public PharmacyController(IPharmacyService pharmacyService)
    {
        _pharmacyService = pharmacyService;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var pharmacies = await _pharmacyService.GetAllAsync();
        return Ok(pharmacies);
    }
    
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var pharmacy = await _pharmacyService.GetByIdAsync(id);

        if (pharmacy == null)
            return NotFound($"Pharmacy with ID {id} not found.");

        return Ok(pharmacy);
    }

    [HttpPost]
    public async Task<IActionResult> AddPharmacy([FromBody] PharmacyDto pharmacyDto)
    {
        if (pharmacyDto == null)
            return BadRequest("Pharmacy data cannot be null.");

        var pharmacy = new Pharmacy
        {
            Name = pharmacyDto.Name,
            Location = pharmacyDto.Location
        };

        await _pharmacyService.AddPharmacyAsync(pharmacy);
        return CreatedAtAction(nameof(GetAll), new { pharmacy_id = pharmacy.PharmacyId }, pharmacy);
    }
    
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePharmacy(int id, [FromBody] PharmacyDto pharmacyDto)
    {
        if (pharmacyDto == null)
            return BadRequest("Pharmacy data cannot be null.");

        var updatedPharmacy = new Pharmacy
        {
            PharmacyId = id,
            Name = pharmacyDto.Name,
            Location = pharmacyDto.Location
        };

        var result = await _pharmacyService.UpdatePharmacyAsync(id, updatedPharmacy);

        if (!result)
            return NotFound($"Pharmacy with ID {id} not found.");

        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePharmacy(int id)
    {
        var result = await _pharmacyService.DeletePharmacyAsync(id);

        if (!result)
            return NotFound($"Pharmacy with ID {id} not found.");

        return NoContent();
    }
}
