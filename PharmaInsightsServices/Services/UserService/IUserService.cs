using PharmaInsightsServices.Models;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllAsync();
    Task AddUserAsync(User user);
    Task<bool> UpdateUserAsync(int id, User updatedUser);
    Task<bool> DeleteUserAsync(int id);
}
