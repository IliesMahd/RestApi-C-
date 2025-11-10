using Microsoft.EntityFrameworkCore;
using RestApi.Entities;
using RestApi.Entities.dto;

namespace RestApi.Services;

public interface IUserService
{
    Task<User> CreateUserAsync(CreateUserDto userDto);
    Task<User?> GetUserByIdAsync(int id);
    Task<List<User>> GetAllUsersAsync();
}

public class UserService: IUserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User> CreateUserAsync(CreateUserDto userDto)
    {
        // Validation de la date de naissance (ne peut pas être dans le futur)
        if (userDto.BirthDate > DateTime.Now)
        {
            throw new Exception("La date de naissance ne peut pas être dans le futur.");
        }

        // Vérifier que l'utilisateur a au moins 18 ans
        var age = DateTime.Now.Year - userDto.BirthDate.Year;
        if (userDto.BirthDate.Date > DateTime.Now.AddYears(-age)) age--;

        if (age < 18)
        {
            throw new Exception("L'utilisateur doit avoir au moins 18 ans.");
        }

        // Créer l'utilisateur
        var user = new User
        {
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            BirthDate = userDto.BirthDate
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users
            .Include(u => u.Accounts)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users
            .Include(u => u.Accounts)
            .ToListAsync();
    }
}