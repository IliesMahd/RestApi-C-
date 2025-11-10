using System.ComponentModel.DataAnnotations;

namespace RestApi.Entities.dto;

public class TransactionDto
{
    [Required(ErrorMessage = "L'identifiant du compte est obligatoire.")]
    public int AccountId { get; set; }

    [Required(ErrorMessage = "Le montant de la transaction est obligatoire.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Le montant de la transaction doit être supérieur à zéro.")]
    public decimal Amount { get; set; }
}