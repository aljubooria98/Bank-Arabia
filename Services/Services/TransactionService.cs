using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace Services.Services;

public class TransactionService
{
    private readonly BankAppDataContext _context;

    public TransactionService(BankAppDataContext context)
    {
        _context = context;
    }

    public enum TransactionType
    {
        Withdraw,
        Deposit,
        Transfer
    }

    public async Task<string?> PerformTransactionAsync(int fromId, int? toId, decimal amount, string type)
    {
        if (!Enum.TryParse<TransactionType>(type, out var transactionType))
            return "❌ Ogiltig transaktionstyp.";

        if (amount <= 0)
            return "❌ Beloppet måste vara större än 0.";

        var fromAccount = await _context.Accounts
            .Include(a => a.Transactions)
            .FirstOrDefaultAsync(a => a.AccountId == fromId);

        if (fromAccount == null)
            return "❌ Från-kontot kunde inte hittas.";

        if ((transactionType == TransactionType.Withdraw || transactionType == TransactionType.Transfer) && fromAccount.Balance < amount)
            return "❌ Otillräckligt saldo på från-kontot.";

        var now = DateTime.Now;

        switch (transactionType)
        {
            case TransactionType.Withdraw:
                fromAccount.Balance -= amount;
                fromAccount.Transactions.Add(new Transaction
                {
                    Amount = -amount,
                    Date = DateOnly.FromDateTime(now),
                    Type = nameof(TransactionType.Withdraw),
                    AccountId = fromId,
                    Balance = fromAccount.Balance
                });
                break;

            case TransactionType.Deposit:
                fromAccount.Balance += amount;
                fromAccount.Transactions.Add(new Transaction
                {
                    Amount = amount,
                    Date = DateOnly.FromDateTime(now),
                    Type = nameof(TransactionType.Deposit),
                    AccountId = fromId,
                    Balance = fromAccount.Balance
                });
                break;

            case TransactionType.Transfer:
                if (!toId.HasValue)
                    return "❌ Till-konto måste anges vid överföring.";

                var toAccount = await _context.Accounts
                    .Include(a => a.Transactions)
                    .FirstOrDefaultAsync(a => a.AccountId == toId.Value);

                if (toAccount == null)
                    return "❌ Till-kontot kunde inte hittas.";

                fromAccount.Balance -= amount;
                toAccount.Balance += amount;

                fromAccount.Transactions.Add(new Transaction
                {
                    Amount = -amount,
                    Date = DateOnly.FromDateTime(now),
                    Type = nameof(TransactionType.Transfer),
                    AccountId = fromId,
                    Balance = fromAccount.Balance,
                    Bank = "INTERNAL"
                });

                toAccount.Transactions.Add(new Transaction
                {
                    Amount = amount,
                    Date = DateOnly.FromDateTime(now),
                    Type = nameof(TransactionType.Transfer),
                    AccountId = toId.Value,
                    Balance = toAccount.Balance,
                    Bank = "INTERNAL"
                });
                break;
        }

        await _context.SaveChangesAsync();
        return null;
    }
}
