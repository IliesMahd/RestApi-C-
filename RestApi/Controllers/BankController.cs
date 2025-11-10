using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestApi.Entities;
using RestApi.Entities.dto;
using RestApi.Services;

namespace RestApi.Controllers;

[ApiController]
[Route("banks")]
[Authorize(Roles = Roles.Admin)]
public class BankController : ControllerBase
{
    private readonly IBankService _bankService;

    public BankController(IBankService bankService)
    {
        _bankService = bankService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateBank([FromBody] CreateBankDto bankDto)
    {
        try
        {
            var bank = await _bankService.CreateBankAsync(bankDto);
            return CreatedAtAction(nameof(GetBank), new { id = bank.Id }, bank);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBank(int id)
    {
        var bank = await _bankService.GetBankByIdAsync(id);
        if (bank == null)
        {
            return NotFound();
        }
        return Ok(bank);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllBanks()
    {
        var banks = await _bankService.GetAllBanksAsync();
        return Ok(banks);
    }
}
