using RestApi.Models.Enums;

namespace RestApi.Models;

public class Transaction
{
    public int Id { get; init; }
    public int AccountId { get; set; }
    public DateTime At { get; set; }
    public TransactionKind Kind { get; set; }
    public decimal Amount { get; set; }
}