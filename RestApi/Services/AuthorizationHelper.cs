using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using RestApi.Entities;
using RestApi.Entities.Enums;

namespace RestApi.Services;

public static class AuthorizationHelper
{
    /// <summary>
    /// Récupère l'ID de l'utilisateur actuel depuis les claims du JWT
    /// </summary>
    public static int GetCurrentUserId(ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Utilisateur non authentifié ou ID invalide.");
        }
        return userId;
    }

    /// <summary>
    /// Vérifie si l'utilisateur actuel possède un rôle spécifique
    /// </summary>
    public static bool IsInRole(ClaimsPrincipal user, UserRole role)
    {
        return user.IsInRole(Roles.ToString(role));
    }

    /// <summary>
    /// Vérifie si l'utilisateur actuel est un administrateur
    /// </summary>
    public static bool IsAdmin(ClaimsPrincipal user)
    {
        return IsInRole(user, UserRole.Admin);
    }

    /// <summary>
    /// Vérifie si l'utilisateur est le propriétaire du compte spécifié
    /// </summary>
    public static async Task<bool> IsAccountOwner(int userId, int accountId, ApplicationDbContext context)
    {
        var account = await context.Accounts
            .FirstOrDefaultAsync(a => a.Id == accountId);

        if (account == null)
        {
            return false;
        }

        // Récupérer l'OwnerId via la navigation property ou directement
        var ownerId = context.Entry(account).Property<int>("OwnerId").CurrentValue;
        return ownerId == userId;
    }

    /// <summary>
    /// Vérifie si l'utilisateur peut accéder à la ressource (propriétaire OU admin)
    /// </summary>
    public static bool CanAccessResource(ClaimsPrincipal user, int resourceOwnerId)
    {
        if (IsAdmin(user))
        {
            return true;
        }

        var currentUserId = GetCurrentUserId(user);
        return currentUserId == resourceOwnerId;
    }
}
