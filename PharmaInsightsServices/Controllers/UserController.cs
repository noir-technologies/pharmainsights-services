using Microsoft.AspNetCore.Mvc;
using PharmaInsightsServices.DTOs;
using PharmaInsightsServices.Models;

namespace PharmaInsightsServices.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllAsync();
        
        var userDtos = users.Select(u => new UserDto
        {
            Name = u.Name,
            Email = u.Email,
            PharmacyId = u.PharmacyId
        });

        return Ok(userDtos);
    }

    [HttpPost]
    public async Task<IActionResult> AddUser([FromBody] UserDto userDto)
    {
        if (userDto == null)
            return BadRequest("User data cannot be null.");

        var user = new User
        {
            Name = userDto.Name,
            Email = userDto.Email,
            PharmacyId = userDto.PharmacyId
        };

        await _userService.AddUserAsync(user);
        return CreatedAtAction(nameof(GetAll), new { user_id = user.UserId }, user);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDto userDto)
    {
        if (userDto == null)
            return BadRequest("User data cannot be null.");

        var updatedUser = new User
        {
            UserId = id,
            Name = userDto.Name,
            Email = userDto.Email,
            PharmacyId = userDto.PharmacyId
        };

        var result = await _userService.UpdateUserAsync(id, updatedUser);

        if (!result)
            return NotFound($"User with ID {id} not found.");

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var result = await _userService.DeleteUserAsync(id);

        if (!result)
            return NotFound($"User with ID {id} not found.");

        return NoContent();
    }
}
