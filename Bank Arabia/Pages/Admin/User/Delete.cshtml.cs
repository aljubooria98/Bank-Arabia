using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bank_Arabia.Pages.Admin.User
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public DeleteModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty(SupportsGet = true)]
        public string Id { get; set; } = "";

        public IdentityUser? UserToDelete { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            UserToDelete = await _userManager.FindByNameAsync(Id);
            if (UserToDelete == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.FindByNameAsync(Id);
            if (user == null) return NotFound();

            await _userManager.DeleteAsync(user);
            return RedirectToPage("/Admin/Users");
        }
    }
}
