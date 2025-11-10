using System.ComponentModel.DataAnnotations;

namespace RestApi.Entities.dto;

public class CreateBankDto
{
    [Required(ErrorMessage = "Le nom de la banque est obligatoire.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Le nom de la banque doit contenir entre 2 et 100 caractères.")]
    public string Name { get; set; }
}
