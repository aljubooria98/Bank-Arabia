using DataAccessLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

public class DataInitializer
{
    public static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        var dbContext = serviceProvider.GetRequiredService<BankAppDataContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await dbContext.Database.MigrateAsync();

        // Skapa rollerna om de inte finns
        string[] roles = { "Admin", "Cashier" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Skapa användare
        await CreateUserAsync(userManager, "richard.chalk@systementor.se", "Hejsan123#", "Admin");
        await CreateUserAsync(userManager, "richard.chalk@customer.systementor.se", "Hejsan123#", "Cashier");
    }

    private static async Task CreateUserAsync(UserManager<IdentityUser> userManager, string email, string password, string role)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
            }
            else
            {
                throw new Exception($"Kunde inte skapa användaren {email}: {string.Join(", ", result.Errors)}");
            }
        }
    }
}
