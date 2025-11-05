using RestApi.Entities;
using RestApi.Models.dto;

namespace RestApi.Services;

public interface IAccountService
{
    Task<Account> CreateAccount(CreateAccountDto dto);
}

public class AccountService: IAccountService
{
    private static int _nextId = 1;

    public AccountService()
    {

    }

    public Task<Account> CreateAccount(CreateAccountDto dto)
    {
        // Mock: créer un user et une bank fictifs
        var mockUser = new User(1, "John", "Doe", new DateTime(1990, 1, 1));
        var mockBank = new Bank(1, "Test Bank");

        // Créer le compte avec les données du DTO
        var account = new Account(
            id: _nextId++,
            bank: mockBank,
            owner: mockUser,
            iban: dto.IBAN,
            balance: dto.Balance
        );

        return Task.FromResult(account);
    }
}