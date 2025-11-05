using RestApi.Entities;
using RestApi.Entities.dto;

namespace RestApi.Services;

public interface ITransactionService
{
    Task<object> Deposit(TransactionDto dto);
    Task<object> Withdraw(TransactionDto dto);
}

public class TransactionService: ITransactionService
{
    // Compte persistant en mémoire
    private static Account _mockAccount;

    public TransactionService()
    {
        // Créer le compte une seule fois au démarrage
        if (_mockAccount == null)
        {
            var mockUser = new User(1, "John", "Doe", new DateTime(1990, 1, 1));
            var mockBank = new Bank(1, "Test Bank");
            _mockAccount = new Account(
                id: 1,
                bank: mockBank,
                owner: mockUser,
                iban: "FR7630006000011234567890189",
                balance: 1000m
            );
        }
    }

    public Task<object> Deposit(TransactionDto dto)
    {
        // Modifier le solde du compte
        _mockAccount.Balance += dto.Amount;

        return Task.FromResult<object>(new
        {
            message = $"Dépôt de {dto.Amount}€ effectué avec succès",
            newBalance = _mockAccount.Balance
        });
    }

    public Task<object> Withdraw(TransactionDto dto)
    {
        // Modifier le solde du compte
        _mockAccount.Balance -= dto.Amount;

        return Task.FromResult<object>(new
        {
            message = $"Retrait de {dto.Amount}€ effectué avec succès",
            newBalance = _mockAccount.Balance
        });
    }
}