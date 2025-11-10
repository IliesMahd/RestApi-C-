namespace RestApi.Entities;

public class RefreshToken
{
    public int Id { get; init; }
    public string Token { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRevoked { get; set; }

    public bool IsExpired => DateTime.Now >= ExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;
}
