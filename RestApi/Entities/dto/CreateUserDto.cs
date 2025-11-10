using System.ComponentModel.DataAnnotations;

namespace RestApi.Entities.dto;

public class CreateUserDto
{
    [Required(ErrorMessage = "Le prénom est obligatoire.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Le prénom doit contenir entre 2 et 100 caractères.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Le nom est obligatoire.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Le nom doit contenir entre 2 et 100 caractères.")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "La date de naissance est obligatoire.")]
    public DateTime BirthDate { get; set; }
}
