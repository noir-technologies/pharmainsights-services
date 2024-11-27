using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PharmaInsightsServices.Data;
using PharmaInsightsServices.DTOs;
using PharmaInsightsServices.Models;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.User.ToListAsync();
    }

    public async Task AddUserAsync(User user)
    {
        var pharmacyExists = await _context.Pharmacy.AnyAsync(p => p.PharmacyId == user.PharmacyId);

        if (!pharmacyExists)
        {
            throw new Exception("The specified PharmacyId does not exist.");
        }

        _context.User.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UpdateUserAsync(int id, User updatedUser)
    {
        var user = await _context.User.FindAsync(id);

        if (user == null)
            return false;

        user.Name = updatedUser.Name;
        user.Email = updatedUser.Email;
        user.PharmacyId = updatedUser.PharmacyId;

        _context.User.Update(user);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _context.User.FindAsync(id);

        if (user == null)
            return false;

        _context.User.Remove(user);
        await _context.SaveChangesAsync();

        return true;
    }
    
    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.User.FirstOrDefaultAsync(u => u.UserId == id);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.User.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> UserExistsAsync(string email)
    {
        return await _context.User.AnyAsync(u => u.Email == email);
    }

    public async Task RegisterAsync(RegisterUserDto registerDto)
    {
        if (await UserExistsAsync(registerDto.Email))
            throw new Exception("User already exists.");

        CreatePasswordHash(registerDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

        var user = new User
        {
            Name = registerDto.Name,
            Email = registerDto.Email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            PharmacyId = registerDto.PharmacyId
        };

        _context.User.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task<User?> LoginAsync(LoginUserDto loginDto)
    {
        var user = await GetUserByEmailAsync(loginDto.Email);

        if (user == null || !VerifyPasswordHash(loginDto.Password, user.PasswordHash, user.PasswordSalt))
            throw new Exception("Invalid email or password.");

        return user;
    }

    public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }

    public bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
    {
        using (var hmac = new HMACSHA512(storedSalt))
        {
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(storedHash);
        }
    }
}
