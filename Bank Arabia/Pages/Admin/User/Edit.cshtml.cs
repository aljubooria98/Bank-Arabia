using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bank_Arabia.Pages.Admin.User
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

        [BindProperty(SupportsGet = true)]
        public string Id { get; set; } = "";

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public List<string> Roles { get; set; } = new();

        public class InputModel
        {
            [Required]
            public string Role { get; set; } = "";
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.FindByNameAsync(Id);
            if (user == null) return NotFound();

            Roles = _roleManager.Roles.Select(r => r.Name!).ToList();
            var currentRoles = await _userManager.GetRolesAsync(user);
            Input.Role = currentRoles.FirstOrDefault() ?? "";

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.FindByNameAsync(Id);
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, Input.Role);

            return RedirectToPage("/Admin/Users");
        }
    }
}