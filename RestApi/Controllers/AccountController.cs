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
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountDto dto)
    {
        var account = await _accountService.CreateAccount(dto);
        return Created("/accounts", new
        {
            message = "Compte créé avec succès",
            data = account
        });
    }
}