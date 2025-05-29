using Microsoft.AspNetCore.Identity;

public static class RoleInitializer
{
    public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roleNames = { "Admin", "User" };

        foreach (var roleName in roleNames)
        {
            bool roleExists = await roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}
