using Microsoft.AspNetCore.Mvc;
using RestApi.Entities.dto;
using RestApi.Services;

namespace RestApi.Controllers;

[ApiController]
[Route("transactions")]
public class TransactionController: ControllerBase
{
    private readonly ITransactionService _transactionService;
    
    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpPost("deposit")]
    public async Task<IActionResult> Deposit([FromBody] TransactionDto dto)
    {
        var result = await _transactionService.Deposit(dto);
        return Ok(result);
    }

    [HttpPost("withdraw")]
    public async Task<IActionResult> Withdraw([FromBody] TransactionDto dto)
    {
        var result = await _transactionService.Withdraw(dto);
        return Ok(result);
    }
}