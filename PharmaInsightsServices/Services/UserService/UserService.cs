using PharmaInsightsServices.Data;
using PharmaInsightsServices.Models;
using Microsoft.EntityFrameworkCore;

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
}
