using Microsoft.EntityFrameworkCore;
using RestApi.Entities;
using RestApi.Entities.dto;

namespace RestApi.Services;

public interface IBankService
{
    Task<Bank> CreateBankAsync(CreateBankDto bankDto);
    Task<Bank?> GetBankByIdAsync(int id);
    Task<List<Bank>> GetAllBanksAsync();
}

public class BankService : IBankService
{
    private readonly ApplicationDbContext _context;

    public BankService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Bank> CreateBankAsync(CreateBankDto bankDto)
    {
        // Vérifier si une banque avec le même nom existe déjà
        var existingBank = await _context.Banks
            .FirstOrDefaultAsync(b => b.Name == bankDto.Name);

        if (existingBank != null)
        {
            throw new Exception($"Une banque avec le nom '{bankDto.Name}' existe déjà.");
        }

        // Créer la banque
        var bank = new Bank
        {
            Name = bankDto.Name
        };

        _context.Banks.Add(bank);
        await _context.SaveChangesAsync();

        return bank;
    }

    public async Task<Bank?> GetBankByIdAsync(int id)
    {
        return await _context.Banks
            .Include(b => b.Accounts)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<List<Bank>> GetAllBanksAsync()
    {
        return await _context.Banks
            .Include(b => b.Accounts)
            .ToListAsync();
    }
}
