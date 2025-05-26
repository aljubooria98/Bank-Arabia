using DataAccessLayer;
using DataAccessLayer.Data;
using DataAccessLayer.Models;
using Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace Bank_Arabia.Web.Pages.Customers
{
    [Authorize(Roles = "Admin,Cashier")]
    public class CreateModel : PageModel
    {
        private readonly BankAppDataContext _context;

        public CreateModel(BankAppDataContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Customer Customer { get; set; } = new();

        public void OnGet()
        {
            // Inga initialiseringar krävs här just nu
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            _context.Customers.Add(Customer);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Search");
        }
    }
}
