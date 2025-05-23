using DataAccessLayer;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bank_Arabia.Pages.Customers
{
    [Authorize(Roles = "Admin,Cashier")]
    public class DetailsModel : PageModel
    {
        private readonly BankAppDataContext _context;
        private readonly ILogger<DetailsModel> _logger;

        public DetailsModel(BankAppDataContext context, ILogger<DetailsModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public CustomerDetailsViewModel? Customer { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (Id <= 0)
            {
                _logger.LogWarning("Ogiltigt kund-ID: {Id}", Id);
                return BadRequest("Ogiltigt kund-ID.");
            }

            var customer = await _context.Customers
                .Include(c => c.Dispositions)
                    .ThenInclude(d => d.Account)

                .FirstOrDefaultAsync(c => c.CustomerId == Id);

            if (customer == null)
            {
                _logger.LogWarning("Kund med ID {Id} hittades inte.", Id);
                return NotFound();
            }

            Customer = new CustomerDetailsViewModel
            {
                CustomerId = customer.CustomerId,
                Name = $"{customer.Givenname} {customer.Surname}",
                Address = customer.Streetaddress,
                Country = customer.Country,
                Phone = $"{customer.Telephonecountrycode}{customer.Telephonenumber}",
                TotalBalance = customer.Accounts.Sum(a => a.Balance),
                Accounts = customer.Accounts
                    .Select(a => new AccountSummaryViewModel
                    {
                        AccountId = a.AccountId,
                        Balance = a.Balance
                    }).ToList()
            };

            return Page();
        }
    }

    public class CustomerDetailsViewModel
    {
        public int CustomerId { get; set; }
        public string Name { get; set; } = "";
        public string? Address { get; set; }
        public string? Country { get; set; }
        public string? Phone { get; set; }
        public decimal TotalBalance { get; set; }
        public List<AccountSummaryViewModel> Accounts { get; set; } = new();
    }

    public class AccountSummaryViewModel
    {
        public int AccountId { get; set; }
        public decimal Balance { get; set; }
    }
}
