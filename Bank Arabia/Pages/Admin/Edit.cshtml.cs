using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bank_Arabia.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public EditModel(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public IdentityUser? User { get; set; }

        [BindProperty]
        public string SelectedRole { get; set; } = "";

        public List<string> AllRoles { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            User = await _userManager.FindByIdAsync(id);
            if (User == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(User);
            SelectedRole = roles.FirstOrDefault() ?? "";

            AllRoles = _roleManager.Roles.Select(r => r.Name!).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || User == null)
                return Page();

            var userToUpdate = await _userManager.FindByIdAsync(User.Id);
            if (userToUpdate == null) return NotFound();

            userToUpdate.UserName = User.UserName;
            userToUpdate.Email = User.Email;

            var updateResult = await _userManager.UpdateAsync(userToUpdate);

            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                AllRoles = _roleManager.Roles.Select(r => r.Name!).ToList();
                return Page();
            }

            // Uppdatera roll
            var currentRoles = await _userManager.GetRolesAsync(userToUpdate);
            await _userManager.RemoveFromRolesAsync(userToUpdate, currentRoles);
            await _userManager.AddToRoleAsync(userToUpdate, SelectedRole);

            return RedirectToPage("Users");
        }
    }
}
