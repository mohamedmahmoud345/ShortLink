using Microsoft.AspNetCore.Identity;
using ShortLink.Infrastructure.Data.Identity;

namespace ShortLink.Infrastructure.Data.SeedingData;

public static class RoleSeeder
{
    public static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
    {
        string[] roles = { "Admin", "User" };

        foreach (var role in roles)
        {
            if (await roleManager.RoleExistsAsync(role))
            {
                continue;
            }

            // Create missing role and fail fast if Identity rejects it.
            var createRoleResult = await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            EnsureSuccess(createRoleResult, $"creating role '{role}'");
        }
    }

    public static async Task SeedAdminUserAsync(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole<Guid>> roleManager,
    string adminEmail,
    string adminPassword)
    {
        if (string.IsNullOrWhiteSpace(adminEmail))
        {
            throw new InvalidOperationException("Seed admin email is required.");
        }

        if (string.IsNullOrWhiteSpace(adminPassword))
        {
            throw new InvalidOperationException("Seed admin password is required.");
        }

        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            // Safety net: make sure Admin role exists even if role seeding order changes.
            var createAdminRoleResult = await roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));
            EnsureSuccess(createAdminRoleResult, "creating Admin role");
        }

        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser is null)
        {
            // Use email as username to keep identity unique and predictable.
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var createAdminResult = await userManager.CreateAsync(adminUser, adminPassword);
            EnsureSuccess(createAdminResult, $"creating admin user '{adminEmail}'");
        }

        // Idempotent repair: ensure the admin user always has Admin role.
        if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
        {
            var addToRoleResult = await userManager.AddToRoleAsync(adminUser, "Admin");
            EnsureSuccess(addToRoleResult, $"adding Admin role to '{adminEmail}'");
        }
    }

    private static void EnsureSuccess(IdentityResult result, string operation)
    {
        if (result.Succeeded)
        {
            return;
        }

        var errors = string.Join("; ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
        throw new InvalidOperationException($"Identity seeding failed while {operation}. {errors}");
    }
}