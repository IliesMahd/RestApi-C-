namespace RestApi.Entities.dto;

public class LoginResponseDto
{
    public string Message { get; set; }
    public int UserId { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
