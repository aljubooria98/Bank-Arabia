using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Services;
using Services.ViewModels;

namespace Bank_Arabia.Pages.Accounts
{
    public class TransactionModel : PageModel
    {
        private readonly TransactionService _transactionService;

        public TransactionModel(TransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [BindProperty]
        public TransactionInputViewModel Input { get; set; } = new();

        public bool IsFromAccountLocked { get; set; } = false;

        public async Task<IActionResult> OnGetAsync(int? fromAccountId)
        {
            if (fromAccountId.HasValue)
            {
                Input.FromAccountId = fromAccountId.Value;
                IsFromAccountLocked = true;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            IsFromAccountLocked = true;

            var result = await _transactionService.PerformTransactionAsync(
                Input.FromAccountId,
                Input.ToAccountId,
                Input.Amount,
                Input.Type);

            if (result != null)
            {
                ModelState.AddModelError(string.Empty, result);
                Message = result;
            }
            else
            {
                Message = "? Transaktionen lyckades!";
            }

            return Page();
        }

        public string? Message { get; set; }
    }
}
