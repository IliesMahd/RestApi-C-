using RestApi.Entities;
using RestApi.Entities.dto;
using RestApi.Entities.Enums;

namespace RestApi.Services;

public interface ITransactionService
{
    Task<Transaction> DepositAsync(TransactionDto dto);
    Task<Transaction> WithdrawAsync(TransactionDto dto);
    
    
}

public class TransactionService: ITransactionService
{
    private readonly ApplicationDbContext _context;

    public TransactionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Transaction> DepositAsync(TransactionDto dto)
    {
        var account = await _context.Accounts.FindAsync(dto.AccountId);
        if (account == null)
        {
            throw new Exception($"Account with ID {dto.AccountId} not found");
        }

        account.Balance += dto.Amount;

        var transaction = new Transaction
        {
            AccountId = dto.AccountId,
            At = DateTime.UtcNow,
            Kind = TransactionKind.Deposit,
            Amount = dto.Amount
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return transaction;
    }

    public async Task<Transaction> WithdrawAsync(TransactionDto dto)
    {
        var account = await _context.Accounts.FindAsync(dto.AccountId);
        if (account == null)
        {
            throw new Exception($"Account with ID {dto.AccountId} not found");
        }

        if (account.Balance < dto.Amount)
        {
            throw new Exception("Insufficient balance");
        }

        account.Balance -= dto.Amount;

        var transaction = new Transaction
        {
            AccountId = dto.AccountId,
            At = DateTime.UtcNow,
            Kind = TransactionKind.Withdraw,
            Amount = dto.Amount
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return transaction;
    }
}