namespace RestApi.Entities;

public class User
{
    public int Id { get; init; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }
    public List<Account> Accounts { get; set; } = new List<Account>();
    
    public User() { }
    
    public User(int id, string firstName, string lastName, DateTime birthDate)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate;
    }
}