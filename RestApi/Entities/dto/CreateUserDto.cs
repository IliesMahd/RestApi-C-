using RestApi.Entities.Enums;

namespace RestApi.Entities.dto;

public class CreateUserDto
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }
    public UserRole? Role { get; set; } // Optionnel, défaut = UserRole.User
}