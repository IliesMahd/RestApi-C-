using System.ComponentModel.DataAnnotations;

namespace RestApi.Entities.dto;
public class CreateAccountDto
{
    [Required(ErrorMessage = "L'identifiant de l'utilisateur est obligatoire.")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "L'identifiant de la banque est obligatoire.")]
    public int BankId { get; set; }

    [Required(ErrorMessage = "L'IBAN est obligatoire.")]
    [StringLength(34, MinimumLength = 15, ErrorMessage = "L'IBAN doit contenir entre 15 et 34 caractères.")]
    public string IBAN { get; set; }
}