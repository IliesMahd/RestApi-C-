using Microsoft.AspNetCore.Mvc;
using RestApi.Entities.dto;
using RestApi.Services;

namespace RestApi.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var (accessToken, refreshToken, user) = await _authService.LoginAsync(loginDto);

            // Configurer le cookie pour l'access token
            SetAccessTokenCookie(accessToken);

            // Configurer le cookie pour le refresh token
            SetRefreshTokenCookie(refreshToken);

            var response = new LoginResponseDto
            {
                Message = "Connexion réussie",
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        try
        {
            // Récupérer le refresh token depuis le cookie
            var refreshToken = Request.Cookies["RefreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(new { message = "Refresh token manquant." });
            }

            var (accessToken, newRefreshToken) = await _authService.RefreshTokenAsync(refreshToken);

            // Mettre à jour les cookies
            SetAccessTokenCookie(accessToken);
            SetRefreshTokenCookie(newRefreshToken);

            return Ok(new { message = "Tokens rafraîchis avec succès" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        try
        {
            // Récupérer le refresh token depuis le cookie
            var refreshToken = Request.Cookies["RefreshToken"];

            if (!string.IsNullOrEmpty(refreshToken))
            {
                // Révoquer le refresh token
                await _authService.RevokeTokenAsync(refreshToken);
            }

            // Supprimer les cookies
            Response.Cookies.Delete("AccessToken");
            Response.Cookies.Delete("RefreshToken");

            return Ok(new { message = "Déconnexion réussie" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private void SetAccessTokenCookie(string accessToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true, // HTTPS uniquement
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddMinutes(30) // Même durée que le JWT
        };

        Response.Cookies.Append("AccessToken", accessToken, cookieOptions);
    }

    private void SetRefreshTokenCookie(string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true, // HTTPS uniquement
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(7) // 7 jours
        };

        Response.Cookies.Append("RefreshToken", refreshToken, cookieOptions);
    }
}
