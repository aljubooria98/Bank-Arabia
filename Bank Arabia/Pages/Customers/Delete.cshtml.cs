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
    public class DeleteModel : PageModel
    {
        private readonly BankAppDataContext _context;

        public DeleteModel(BankAppDataContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Customer Customer { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Customer = await _context.Customers.FindAsync(id);
            if (Customer == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var customerToDelete = await _context.Customers.FindAsync(Customer.CustomerId);
            if (customerToDelete == null)
                return NotFound();

            _context.Customers.Remove(customerToDelete);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Search");
        }
    }
}
