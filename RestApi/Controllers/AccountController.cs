using Microsoft.AspNetCore.Mvc;
using RestApi.Entities.dto;
using RestApi.Services;

namespace RestApi.Controllers;

[ApiController]
[Route("accounts")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    
    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }
    
    [HttpPost]
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
    public async Task<IActionResult> GetAccount(int id)
    {
        var account = await _accountService.GetAccountByIdAsync(id);
        if (account == null)
        {
            return NotFound();
        }
        return Ok(account);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAccounts()
    {
        var accounts = await _accountService.GetAllAccountsAsync();
        return Ok(accounts);
    }
}