using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Services;
using Services.ViewModels;

namespace Bank_Arabia.Pages.Accounts
{
    public class DetailsModel : PageModel
    {
        private readonly AccountService _accountService;

        public DetailsModel(AccountService accountService)
        {
            _accountService = accountService;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public AccountDetailsViewModel? Account { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Account = await _accountService.GetAccountDetailsAsync(Id, 0, 20);

            if (Account == null)
                return NotFound();

            return Page();
        }
    }
}
