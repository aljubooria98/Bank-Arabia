using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bank_Arabia.Pages.Admin.User
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CreateModel(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public List<string> Roles { get; private set; } = new();

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "Lösenordet måste vara minst 6 tecken.")]
            public string Password { get; set; } = string.Empty;

            [Required(ErrorMessage = "Du måste välja en roll.")]
            public string Role { get; set; } = string.Empty;
        }

        public async Task OnGetAsync()
        {
            Roles = await GetAllRolesAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Roles = await GetAllRolesAsync();

            if (!ModelState.IsValid)
                return Page();

            var user = new IdentityUser
            {
                UserName = Input.Email,
                Email = Input.Email,
                EmailConfirmed = true // e-postbekräftelse direkt – justera beroende på policy
            };

            var result = await _userManager.CreateAsync(user, Input.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return Page();
            }

            if (!await _roleManager.RoleExistsAsync(Input.Role))
            {
                ModelState.AddModelError(string.Empty, $"Rollen '{Input.Role}' finns inte.");
                return Page();
            }

            await _userManager.AddToRoleAsync(user, Input.Role);

            return RedirectToPage("./Index"); // säkrare routing
        }

        private async Task<List<string>> GetAllRolesAsync()
        {
            return await Task.FromResult(_roleManager.Roles
                .Select(r => r.Name!)
                .OrderBy(name => name)
                .ToList());
        }
    }
}
