using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RestApi.Entities;
using RestApi.Entities.dto;

namespace RestApi.Services;

public interface IAuthService
{
    Task<(string accessToken, string refreshToken, User user)> LoginAsync(LoginDto loginDto);
    Task<(string accessToken, string refreshToken)> RefreshTokenAsync(string refreshToken);
    Task RevokeTokenAsync(string refreshToken);
}

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AuthService(
        ApplicationDbContext context,
        ITokenService tokenService,
        IConfiguration configuration,
        UserManager<User> userManager,
        SignInManager<User> signInManager)
    {
        _context = context;
        _tokenService = tokenService;
        _configuration = configuration;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<(string accessToken, string refreshToken, User user)> LoginAsync(LoginDto loginDto)
    {
        // Vérifier si l'utilisateur existe
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null)
        {
            throw new Exception("Email ou mot de passe incorrect.");
        }

        // Vérifier le mot de passe avec SignInManager
        // lockoutOnFailure: true active le verrouillage après plusieurs tentatives échouées
        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
            {
                throw new Exception("Compte verrouillé en raison de tentatives de connexion échouées.");
            }
            throw new Exception("Email ou mot de passe incorrect.");
        }

        // Générer les tokens
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Sauvegarder le refresh token en base de données
        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.Now.AddDays(7), // 7 jours d'expiration
            CreatedAt = DateTime.Now,
            IsRevoked = false
        };

        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync();

        return (accessToken, refreshToken, user);
    }

    public async Task<(string accessToken, string refreshToken)> RefreshTokenAsync(string refreshToken)
    {
        // Vérifier si le refresh token existe et est valide
        var storedToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (storedToken == null)
        {
            throw new Exception("Refresh token invalide.");
        }

        if (!storedToken.IsActive)
        {
            throw new Exception("Refresh token expiré ou révoqué.");
        }

        // Révoquer l'ancien refresh token
        storedToken.IsRevoked = true;

        // Générer de nouveaux tokens
        var accessToken = _tokenService.GenerateAccessToken(storedToken.User);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        // Créer un nouveau refresh token
        var newRefreshTokenEntity = new RefreshToken
        {
            Token = newRefreshToken,
            UserId = storedToken.UserId,
            ExpiresAt = DateTime.Now.AddDays(7),
            CreatedAt = DateTime.Now,
            IsRevoked = false
        };

        _context.RefreshTokens.Add(newRefreshTokenEntity);
        await _context.SaveChangesAsync();

        return (accessToken, newRefreshToken);
    }

    public async Task RevokeTokenAsync(string refreshToken)
    {
        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (storedToken == null)
        {
            throw new Exception("Refresh token invalide.");
        }

        storedToken.IsRevoked = true;
        await _context.SaveChangesAsync();
    }
}
