namespace RestApi.Models;

public class Account
{
    public int Id { get; init; }
    public Bank Bank { get; set; }
    public User Owner { get; set; }
    public string IBAN { get; set; }
    public decimal Balance { get; set; }
    
    public Account() { }
    
    public Account(int id, Bank bank, User owner, string iban, decimal balance)
    {
        Id = id;
        Bank = bank;
        Owner = owner;
        IBAN = iban;
        Balance = balance;
        
        // TODO : Établir les relations bidirectionnelles pour ajouter le compte à la banque et à l'utilisateur
    }
}