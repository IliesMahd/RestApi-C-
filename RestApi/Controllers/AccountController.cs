using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestApi.Entities;
using RestApi.Entities.dto;
using RestApi.Services;

namespace RestApi.Controllers;

[ApiController]
[Route("accounts")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly ApplicationDbContext _context;

    public AccountController(IAccountService accountService, ApplicationDbContext context)
    {
        _accountService = accountService;
        _context = context;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountDto accountDto)
    {
        try
        {
            var account = await _accountService.CreateAccountAsync(accountDto);
            return CreatedAtAction(nameof(GetAccount), new { id = account.Id }, account);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetAccount(int id)
    {
        var account = await _accountService.GetAccountByIdAsync(id);
        if (account == null)
        {
            return NotFound();
        }

        // Vérifier si l'utilisateur est le propriétaire du compte ou admin
        var currentUserId = AuthorizationHelper.GetCurrentUserId(User);
        var isOwner = await AuthorizationHelper.IsAccountOwner(currentUserId, id, _context);
        var isAdmin = AuthorizationHelper.IsAdmin(User);

        if (!isOwner && !isAdmin)
        {
            return Forbid();
        }

        return Ok(account);
    }

    [HttpGet]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> GetAllAccounts()
    {
        var accounts = await _accountService.GetAllAccountsAsync();
        return Ok(accounts);
    }
}