using DataAccessLayer.Data;
using Microsoft.EntityFrameworkCore;
using TransactionModel = DataAccessLayer.Models.Transaction;

namespace Services.Services;

public class TransactionService
{
    private readonly BankAppDataContext _context;

    public TransactionService(BankAppDataContext context)
    {
        _context = context;
    }

    public async Task<string?> PerformTransactionAsync(int fromId, int? toId, decimal amount, string type)
    {
        if (amount <= 0)
            return "❌ Beloppet måste vara större än 0.";

        var fromAccount = await _context.Accounts
            .Include(a => a.Transactions)
            .FirstOrDefaultAsync(a => a.AccountId == fromId);

        if (fromAccount == null)
            return "❌ Från-kontot kunde inte hittas.";

        if (type == "Withdraw" || type == "Transfer")
        {
            if (fromAccount.Balance < amount)
                return "❌ Otillräckligt saldo på från-kontot.";
        }

        switch (type)
        {
            case "Withdraw":
                fromAccount.Balance -= amount;
                fromAccount.Transactions.Add(new TransactionModel
                {
                    Amount = -amount,
                    Date = DateOnly.FromDateTime(DateTime.Today),
                    Type = "Withdraw",
                    AccountId = fromId,
                    Operation = "",
                    Balance = fromAccount.Balance
                });
                break;

            case "Deposit":
                fromAccount.Balance += amount;
                fromAccount.Transactions.Add(new TransactionModel
                {
                    Amount = amount,
                    Date = DateOnly.FromDateTime(DateTime.Today),
                    Type = "Deposit",
                    AccountId = fromId,
                    Operation = "",
                    Balance = fromAccount.Balance
                });
                break;

            case "Transfer":
                if (!toId.HasValue)
                    return "❌ Till-konto måste anges vid överföring.";

                var toAccount = await _context.Accounts
                    .Include(a => a.Transactions)
                    .FirstOrDefaultAsync(a => a.AccountId == toId.Value);

                if (toAccount == null)
                    return "❌ Till-kontot kunde inte hittas.";

                fromAccount.Balance -= amount;
                toAccount.Balance += amount;

                fromAccount.Transactions.Add(new TransactionModel
                {
                    Amount = -amount,
                    Date = DateOnly.FromDateTime(DateTime.Today),
                    Type = "Transfer",
                    AccountId = fromId,
                    Operation = "",
                    Balance = fromAccount.Balance
                });

                toAccount.Transactions.Add(new TransactionModel
                {
                    Amount = amount,
                    Date = DateOnly.FromDateTime(DateTime.Today),
                    Type = "Transfer",
                    AccountId = toId.Value,
                    Operation = "",
                    Balance = toAccount.Balance
                });
                break;

            default:
                return "❌ Ogiltig transaktionstyp.";
        }

        await _context.SaveChangesAsync();
        return null;
    }
}
