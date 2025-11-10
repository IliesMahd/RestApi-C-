using Microsoft.EntityFrameworkCore;
using RestApi.Entities;
using RestApi.Entities.dto;

namespace RestApi.Services;

public interface IAccountService
{
    Task<Account> CreateAccountAsync(CreateAccountDto accountDto);
    Task<Account?> GetAccountByIdAsync(int id);
    Task<List<Account>> GetAllAccountsAsync();
}

public class AccountService: IAccountService
{
    // private static int _nextId = 1;

    private readonly ApplicationDbContext _context;
    
    public AccountService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Account> CreateAccountAsync(CreateAccountDto accountDto)
    {
        // Vérifier si l'IBAN existe déjà
        var existingAccount = await _context.Accounts
            .FirstOrDefaultAsync(a => a.IBAN == accountDto.IBAN);

        if (existingAccount != null)
        {
            throw new Exception($"Un compte avec l'IBAN '{accountDto.IBAN}' existe déjà.");
        }

        // Récupérer l'utilisateur depuis la base de données
        var user = await _context.Users.FindAsync(accountDto.UserId);
        if (user == null)
        {
            throw new Exception($"User with ID {accountDto.UserId} not found");
        }

        // Récupérer la banque depuis la base de données
        var bank = await _context.Banks.FindAsync(accountDto.BankId);
        if (bank == null)
        {
            throw new Exception($"Bank with ID {accountDto.BankId} not found");
        }

        // Créer le compte
        var account = new Account
        {
            Bank = bank,
            Owner = user,
            IBAN = accountDto.IBAN,
            Balance = 0
        };

        // Ajouter à la base de données
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        return account;
    }
    
    public async Task<Account?> GetAccountByIdAsync(int id)
    {
        return await _context.Accounts
            .Include(a => a.Owner)
            .Include(a => a.Bank)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<List<Account>> GetAllAccountsAsync()
    {
        return await _context.Accounts
            .Include(a => a.Owner)
            .Include(a => a.Bank)
            .ToListAsync();
    }
}