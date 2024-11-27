using Microsoft.AspNetCore.Authorization;
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
    
    [Authorize]
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

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddUser([FromBody] RegisterUserDto userDto)
    {
        if (userDto == null)
            return BadRequest("User data cannot be null.");

        // Generate password hash and salt
        _userService.CreatePasswordHash(userDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

        var user = new User
        {
            Name = userDto.Name,
            Email = userDto.Email,
            PharmacyId = userDto.PharmacyId,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt
        };

        await _userService.AddUserAsync(user);
        return CreatedAtAction(nameof(GetAll), new { user_id = user.UserId }, user);
    }
    
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDto userDto)
    {
        if (userDto == null)
            return BadRequest("User data cannot be null.");

        var existingUser = await _userService.GetUserByIdAsync(id);

        if (existingUser == null)
            return NotFound($"User with ID {id} not found.");

        // Update password only if provided
        byte[] passwordHash = existingUser.PasswordHash;
        byte[] passwordSalt = existingUser.PasswordSalt;

        if (!string.IsNullOrEmpty(userDto.Password))
        {
            _userService.CreatePasswordHash(userDto.Password, out passwordHash, out passwordSalt);
        }

        var updatedUser = new User
        {
            UserId = id,
            Name = userDto.Name,
            Email = userDto.Email,
            PharmacyId = userDto.PharmacyId,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt
        };

        var result = await _userService.UpdateUserAsync(id, updatedUser);

        if (!result)
            return NotFound($"User with ID {id} not found.");

        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var result = await _userService.DeleteUserAsync(id);

        if (!result)
            return NotFound($"User with ID {id} not found.");

        return NoContent();
    }
}
