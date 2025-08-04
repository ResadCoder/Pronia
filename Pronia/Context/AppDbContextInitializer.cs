using Microsoft.AspNetCore.Identity;
using Pronia.Extensions;
using Pronia.Models;

namespace Pronia.Context;

public class AppDbContextInitializer(RoleManager<IdentityRole<int>> roleManager, UserManager<User> userManager,IConfiguration configuration)
{
    public  async Task InitializeAsync()
    {
        await CreateRolesAsync();
        await CreateAdminAsync();
    }

    private async Task CreateRolesAsync()
    {
        foreach (UserRoleEnum  role in  Enum.GetValues<UserRoleEnum>())
        {
            if(!await roleManager.RoleExistsAsync(role.ToString()))
            {
                await roleManager.CreateAsync(new IdentityRole<int>(role.ToString()));
            }
        }
    }

    private async Task CreateAdminAsync()
    {
        User? admin = await userManager.FindByEmailAsync(configuration["Admin:Email"]);
        if (admin == null)
        {
            admin = new User
            {
                Email = configuration["Admin:Email"],
                UserName = configuration["Admin:UserName"],
                Name = "Rashad",
                Surname = "Sadigov",
                EmailConfirmed = true,
            };
            var res = await userManager.CreateAsync(admin, configuration["Admin:Password"]!);
            if (res.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, nameof(UserRoleEnum.Admin));
            }
        }
    }
}