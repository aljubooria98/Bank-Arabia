using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Services.ViewModels;
using DataAccessLayer.Data;

namespace Services.Services;

public class StatisticsService
{
    private readonly BankAppDataContext _context;

    public StatisticsService(BankAppDataContext context)
    {
        _context = context;
    }

    public async Task<StatisticsViewModel> GetStatisticsAsync()
    {
        var customerCount = await _context.Customers.CountAsync();
        var accountCount = await _context.Accounts.CountAsync();

        // För att undvika problem med nullreferenser används Join mellan Customers och Dispositions → Accounts
        var balancePerCountry = await _context.Customers
            .Include(c => c.Dispositions)
                .ThenInclude(d => d.Account)
            .GroupBy(c => c.Country)
            .Select(g => new
            {
                Country = g.Key ?? "Okänt",
                Total = g.SelectMany(c => c.Dispositions)
                         .Where(d => d.Account != null)
                         .Sum(d => d.Account.Balance)
            })
            .ToDictionaryAsync(x => x.Country, x => x.Total);

        return new StatisticsViewModel
        {
            CustomerCount = customerCount,
            AccountCount = accountCount,
            BalancePerCountry = balancePerCountry
        };
    }
}
