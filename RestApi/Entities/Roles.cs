using RestApi.Entities.Enums;

namespace RestApi.Entities;

public static class Roles
{
    public const string User = nameof(UserRole.User);
    public const string Admin = nameof(UserRole.Admin);

    /// <summary>
    /// Convertit un UserRole en string pour Identity
    /// </summary>
    public static string ToString(UserRole role) => role.ToString();

    /// <summary>
    /// Convertit un string en UserRole
    /// </summary>
    public static UserRole FromString(string role) => Enum.Parse<UserRole>(role);
}
