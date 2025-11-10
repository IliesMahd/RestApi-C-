using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestApi.Entities;
using RestApi.Entities.dto;
using RestApi.Services;

namespace RestApi.Controllers;

[ApiController]
[Route("transactions")]
public class TransactionController: ControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly ApplicationDbContext _context;

    public TransactionController(ITransactionService transactionService, ApplicationDbContext context)
    {
        _transactionService = transactionService;
        _context = context;
    }

    [HttpPost("deposit")]
    [Authorize]
    public async Task<IActionResult> Deposit([FromBody] TransactionDto dto)
    {
        // Vérifier que le compte appartient à l'utilisateur (sauf si admin)
        var currentUserId = AuthorizationHelper.GetCurrentUserId(User);
        var isOwner = await AuthorizationHelper.IsAccountOwner(currentUserId, dto.AccountId, _context);
        var isAdmin = AuthorizationHelper.IsAdmin(User);

        if (!isOwner && !isAdmin)
        {
            return Forbid();
        }

        try
        {
            var result = await _transactionService.DepositAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("withdraw")]
    [Authorize]
    public async Task<IActionResult> Withdraw([FromBody] TransactionDto dto)
    {
        // Vérifier que le compte appartient à l'utilisateur (sauf si admin)
        var currentUserId = AuthorizationHelper.GetCurrentUserId(User);
        var isOwner = await AuthorizationHelper.IsAccountOwner(currentUserId, dto.AccountId, _context);
        var isAdmin = AuthorizationHelper.IsAdmin(User);

        if (!isOwner && !isAdmin)
        {
            return Forbid();
        }

        try
        {
            var result = await _transactionService.WithdrawAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}