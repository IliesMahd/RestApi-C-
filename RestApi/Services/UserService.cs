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
        // Vérifier si l'email existe déjà
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == userDto.Email);
        if (existingUser != null)
        {
            throw new Exception("Un utilisateur avec cet email existe déjà.");
        }

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

        // Hasher le mot de passe
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

        // Créer l'utilisateur
        var user = new User
        {
            Email = userDto.Email,
            PasswordHash = passwordHash,
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