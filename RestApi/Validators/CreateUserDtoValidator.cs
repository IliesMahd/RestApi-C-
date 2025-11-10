using FluentValidation;
using RestApi.Entities.dto;

namespace RestApi.Validators;

public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("L'email est obligatoire.")
            .EmailAddress().WithMessage("L'email n'est pas valide.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Le mot de passe est obligatoire.")
            .MinimumLength(8).WithMessage("Le mot de passe doit contenir au moins 8 caractères.")
            .Matches("[a-z]").WithMessage("Le mot de passe doit contenir au moins une lettre minuscule.")
            .Matches("[A-Z]").WithMessage("Le mot de passe doit contenir au moins une lettre majuscule.")
            .Matches(@"\d").WithMessage("Le mot de passe doit contenir au moins un chiffre.")
            .Matches(@"[@$!%*?&]").WithMessage("Le mot de passe doit contenir au moins un caractère spécial (@$!%*?&).")
            .Matches(@"^[A-Za-z\d@$!%*?&]+$").WithMessage("Le mot de passe ne peut contenir que des lettres, chiffres et caractères spéciaux (@$!%*?&).");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Le prénom est obligatoire.")
            .Length(2, 100).WithMessage("Le prénom doit contenir entre 2 et 100 caractères.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Le nom est obligatoire.")
            .Length(2, 100).WithMessage("Le nom doit contenir entre 2 et 100 caractères.");

        RuleFor(x => x.BirthDate)
            .NotEmpty().WithMessage("La date de naissance est obligatoire.");
    }
}
