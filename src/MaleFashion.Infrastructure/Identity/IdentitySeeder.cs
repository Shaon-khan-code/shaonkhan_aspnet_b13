using MaleFashion.Domain.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MaleFashion.Infrastructure.Identity;

public static class IdentitySeeder
{
    public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = { RoleNames.Admin, RoleNames.Customer };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    public static async Task SeedAdminUserAsync(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        ILogger logger)
    {
        var email = configuration["AdminUser:Email"];
        var password = configuration["AdminUser:Password"];
        var fullName = configuration["AdminUser:FullName"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning(
                "Admin user seeding skipped: AdminUser:Email or AdminUser:Password is not configured.");
            return;
        }

        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            return;
        }

        var adminUser = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = fullName,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, password);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, RoleNames.Admin);
        }
        else
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            logger.LogError("Failed to create default admin user: {Errors}", errors);
        }
    }
}