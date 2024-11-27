using PharmaInsightsServices.DTOs;
using PharmaInsightsServices.Models;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllAsync();
    Task AddUserAsync(User user);
    Task<bool> UpdateUserAsync(int id, User updatedUser);
    Task<bool> DeleteUserAsync(int id);
    Task<User?> GetUserByIdAsync(int id);
    Task<User?> GetUserByEmailAsync(string email);
    Task<bool> UserExistsAsync(string email);
    void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
    bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt);
    Task RegisterAsync(RegisterUserDto registerDto);
    Task<User?> LoginAsync(LoginUserDto loginDto);
}
