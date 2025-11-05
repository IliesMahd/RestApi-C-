
namespace RestApi.Entities;

public class Bank
{
    public int Id { get; init; }
    public string Name { get; set; }
    public List<Account> Accounts { get; set; } = new List<Account>();
    public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    
    public Bank() { }
    
    public Bank(int id, string name)
    {
        Id = id;
        Name = name;
    }
}