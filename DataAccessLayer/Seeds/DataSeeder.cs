using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace DataAccessLayer.Seeds
{
    public static class DataSeeder
    {
        public static async Task SeedUsersAndRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string[] roles = { "Admin", "Cashier" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var roleResult = await roleManager.CreateAsync(new IdentityRole(role));
                    if (!roleResult.Succeeded)
                    {
                        throw new Exception($"Failed to create role {role}: {string.Join(", ", roleResult.Errors)}");
                    }
                }
            }

            await CreateUserIfNotExists(userManager,
                email: "richard.chalk@admin.se",
                username: "admin_user",
                password: "Abc123#",
                role: "Admin");

            await CreateUserIfNotExists(userManager,
                email: "richard.chalk@cashier.se",
                username: "cashier_user",
                password: "Abc123#",
                role: "Cashier");
        }

        private static async Task CreateUserIfNotExists(
            UserManager<IdentityUser> userManager,
            string email,
            string username,
            string password,
            string role)
        {
            var existingUser = await userManager.FindByEmailAsync(email);
            if (existingUser == null)
            {
                var user = new IdentityUser
                {
                    Email = email,
                    UserName = username,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(user, password);
                if (createResult.Succeeded)
                {
                    var addRoleResult = await userManager.AddToRoleAsync(user, role);
                    if (!addRoleResult.Succeeded)
                    {
                        throw new Exception($"Failed to assign role '{role}' to user '{username}': {string.Join(", ", addRoleResult.Errors)}");
                    }
                }
                else
                {
                    throw new Exception($"Failed to create user '{username}': {string.Join(", ", createResult.Errors)}");
                }
            }
        }
    }
}