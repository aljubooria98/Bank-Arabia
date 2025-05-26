using DataAccessLayer.Data;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bank_Arabia.Web.Pages.Customers
{
    [Authorize(Roles = "Admin,Cashier")]
    public class EditModel : PageModel
    {
        private readonly BankAppDataContext _context;

        public EditModel(BankAppDataContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Customer Customer { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == id);

            if (Customer == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var existingCustomer = await _context.Customers.FindAsync(Customer.CustomerId);

            if (existingCustomer == null)
                return NotFound();

            // Uppdatera fält
            existingCustomer.Givenname = Customer.Givenname;
            existingCustomer.Surname = Customer.Surname;
            existingCustomer.Streetaddress = Customer.Streetaddress;
            existingCustomer.City = Customer.City;
            existingCustomer.Zipcode = Customer.Zipcode;
            existingCustomer.Country = Customer.Country;
            existingCustomer.Telephonecountrycode = Customer.Telephonecountrycode;
            existingCustomer.Telephonenumber = Customer.Telephonenumber;
            existingCustomer.Emailaddress = Customer.Emailaddress;

            await _context.SaveChangesAsync();

            return RedirectToPage("Search");
        }
    }
}
