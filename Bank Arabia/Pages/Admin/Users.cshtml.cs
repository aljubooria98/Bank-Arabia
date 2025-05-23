using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.ViewModels;

namespace Bank_Arabia.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class UsersModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UsersModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public List<UserWithRolesViewModel> Users { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(currentUser!);

            if (!roles.Contains("Admin"))
            {
                return Forbid(); 
            }

            var allUsers = _userManager.Users.ToList();

            foreach (var user in allUsers)
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                Users.Add(new UserWithRolesViewModel
                {
                    Id = user.Id,
                    Email = user.Email ?? "",
                    UserName = user.UserName ?? "",
                    Roles = userRoles.ToList()
                });
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }

            return RedirectToPage();
        }
    }
}