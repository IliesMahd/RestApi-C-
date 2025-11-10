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
    public async Task<IActionResult> Withdraw([FromBody] TransactionDto dto)
    {
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