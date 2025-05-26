using DataAccessLayer;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.ViewModels;
using Services.Services;

namespace Bank_Arabia.Pages.Customers
{
    [Authorize(Roles = "Admin,Cashier")]
    public class DetailsModel : PageModel
    {
        private readonly CustomerService _customerService;

        public DetailsModel(CustomerService customerService)
        {
            _customerService = customerService;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public CustomerDetailsViewModel? Customer { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Customer = await _customerService.GetCustomerDetailsAsync(Id);

            if (Customer == null)
                return NotFound();

            return Page();
        }
    }
}
