using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Services.ViewModels;
using DataAccessLayer.Models;

namespace Services.Services;

public class AccountService
{
    private readonly BankAppDataContext _context;

    public AccountService(BankAppDataContext context)
    {
        _context = context;
    }

    public async Task<AccountDetailsViewModel?> GetAccountDetailsAsync(int accountId, int skip = 0, int take = 20)
    {
        var account = await _context.Accounts
            .Include(a => a.Transactions)
            .FirstOrDefaultAsync(a => a.AccountId == accountId);

        if (account == null)
            return null;

        var transactions = account.Transactions
            .OrderByDescending(t => t.Date)
            .Skip(skip)
            .Take(take)
            .Select(t => new TransactionViewModel
            {
                TransactionId = t.TransactionId,
                Amount = t.Amount,
                Date = t.Date.ToDateTime(TimeOnly.MinValue),
                Operation = t.Operation ?? ""
            })
            .ToList();

        return new AccountDetailsViewModel
        {
            AccountId = account.AccountId,
            Balance = account.Balance,
            Transactions = transactions
        };
    }
}
