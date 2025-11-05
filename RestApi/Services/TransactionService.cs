using RestApi.Entities.dto;

namespace RestApi.Services;

public interface ITransactionService
{
    Task<object> Deposit(TransactionDto dto);
    Task<object> Withdraw(TransactionDto dto);
}

public class TransactionService: ITransactionService
{
    public Task<object> Deposit(TransactionDto dto)
    {
        // Mock: créer un compte lambda fictif
        var mockAccountId = 1;
        var currentBalance = 1000m; // Solde fictif du compte
        var newBalance = currentBalance + dto.Amount;

        return Task.FromResult<object>(new
        {
            message = $"Dépôt de {dto.Amount}€ effectué avec succès",
            newBalance
        });
    }

    public Task<object> Withdraw(TransactionDto dto)
    {
        // Mock: créer un compte lambda fictif
        var mockAccountId = 1;
        var currentBalance = 1000m; // Solde fictif du compte
        var newBalance = currentBalance - dto.Amount;

        return Task.FromResult<object>(new
        {
            message = $"Retrait de {dto.Amount}€ effectué avec succès",
            newBalance
        });
    }
}