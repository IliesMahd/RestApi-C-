using System.ComponentModel.DataAnnotations;

namespace RestApi.Models.dto;

public class CreateAccountDto
{
    [Required(ErrorMessage = "L'IBAN est obligatoire.")]
    [StringLength(34, MinimumLength = 15, ErrorMessage = "L'IBAN doit contenir entre 15 et 34 caractères.")]
    public string IBAN { get; set;  }
    
    [Required(ErrorMessage = "Le solde initial est obligatoire.")]
    [Range(0, double.MaxValue, ErrorMessage = "Le solde initial doit être un montant positif.")]
    public decimal Balance { get; set; }
}