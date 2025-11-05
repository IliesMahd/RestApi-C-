namespace RestApi.Services;

public interface IAccountService
{
    Task CreateAccount();
}

public class AccountService: IAccountService
{
    public AccountService()
    {
        
    }
    
    public async Task CreateAccount()
    {
        // fake delay to simulate async work
        Console.WriteLine("Creating account...");
        await Task.Delay(100);
    }
}