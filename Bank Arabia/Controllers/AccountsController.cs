using Services.ViewModels;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Services.Services;

[Route("api/accounts")]
[ApiController]
public class AccountsController : ControllerBase
{
    private readonly AccountService _accountService;

    public AccountsController(AccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet("{id}/transactions")]
    public async Task<IActionResult> GetTransactions(int id, int skip = 0, int take = 20)
    {
        var result = await _accountService.GetAccountDetailsAsync(id, skip, take);
        if (result == null)
            return NotFound();

        var data = result.Transactions.Select(t => new
        {
            date = t.Date.ToString("yyyy-MM-dd HH:mm"),
            amount = t.Amount.ToString("C")
        });

        return Ok(data);
    }
}
