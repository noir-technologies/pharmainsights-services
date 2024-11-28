using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaInsightsServices.DTOs;
using PharmaInsightsServices.Helpers;
using PharmaInsightsServices.Models;

namespace PharmaInsightsServices.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly JwtHelper _jwtHelper;

    public AuthController(IUserService userService, JwtHelper jwtHelper)
    {
        _userService = userService;
        _jwtHelper = jwtHelper;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
    {
        if (registerDto == null)
            return BadRequest(new { error = "Request body cannot be null." });

        try
        {
            await _userService.RegisterAsync(registerDto);
            return Ok(new { message = "User registered successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto loginDto)
    {
        if (loginDto == null)
            return BadRequest(new { error = "Request body cannot be null." });

        try
        {
            var user = await _userService.GetUserByEmailAsync(loginDto.Email);
            
            if (user == null || !_userService.VerifyPasswordHash(loginDto.Password, user.PasswordHash, user.PasswordSalt))
                throw new Exception("Invalid email or password.");

            // Generate JWT Token
            var token = _jwtHelper.GenerateJwtToken(user);

            return Ok(new
            {
                token,
                user = new
                {
                    user.UserId,
                    user.Name,
                    user.Email,
                    user.PharmacyId,
                    user.PasswordHash,
                    user.PasswordSalt,
                }
            });
        }
        catch (Exception ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }
    
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetUserDetails()
    {
        try
        {
            // Log all claims for debugging
            Console.WriteLine("User Claims:");
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"Type: {claim.Type}, Value: {claim.Value}");
            }

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                Console.WriteLine("No 'nameidentifier' claim found");
                return Unauthorized(new { error = "Invalid token." });
            }

            var userId = int.Parse(userIdClaim.Value);
            var user = await _userService.GetUserByIdAsync(userId);

            if (user == null)
            {
                Console.WriteLine("User not found for UserId: " + userId);
                return Unauthorized(new { error = "User not found." });
            }

            return Ok(new
            {
                user.UserId,
                user.Name,
                user.Email,
                user.PharmacyId
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in 'me' endpoint: " + ex.Message);
            return BadRequest(new { error = ex.Message });
        }
    }
}
