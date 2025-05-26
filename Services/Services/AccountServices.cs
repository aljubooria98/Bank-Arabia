using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Services.ViewModels;
using DataAccessLayer.Data;

namespace Services.Services;

    public class AccountService
{
    private readonly BankAppDataContext _context;

    public AccountService(BankAppDataContext context)
    {
        _context = context;
    }

    public async Task<AccountDetailsViewModel?> GetAccountDetailsAsync(int accountId, int page = 1, int pageSize = 10)
    {
        // Säkerställ att page inte blir mindre än 1
        if (page < 1) page = 1;

        var account = await _context.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.AccountId == accountId);

        if (account == null)
            return null;

        // ... resten som tidigare


        var totalTransactions = await _context.Transactions
            .CountAsync(t => t.AccountId == accountId);

        var totalPages = (int)Math.Ceiling(totalTransactions / (double)pageSize);

        var transactions = await _context.Transactions
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TransactionViewModel
            {
                TransactionId = t.TransactionId,
                Amount = t.Amount,
                Date = t.Date.ToDateTime(TimeOnly.MinValue),
                Operation = t.Type
            })
            .ToListAsync();

        return new AccountDetailsViewModel
        {
            AccountId = account.AccountId,
            Balance = account.Balance,
            Transactions = transactions,
            Page = page,
            TotalPages = totalPages
        };
    }

    public async Task<List<TransactionViewModel>> GetTransactionsAsync(int accountId, int page = 1, int pageSize = 20)
    {
        return await _context.Transactions
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TransactionViewModel
            {
                TransactionId = t.TransactionId,
                Amount = t.Amount,
                Date = t.Date.ToDateTime(TimeOnly.MinValue),
                Operation = t.Type
            })
            .ToListAsync();
    }
}

