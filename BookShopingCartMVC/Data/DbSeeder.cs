using BookShopingCartMVC.Constants;
using Microsoft.AspNetCore.Identity;

namespace BookShopingCartMVC.Data
{
    public class DbSeeder
    {

        public static async Task SeedDefault(IServiceProvider service)
        {
            var userManager = service.GetService<UserManager<IdentityUser>>();
            var roleManager = service.GetService<RoleManager<IdentityRole>>();
            // add some roles to Database
            var roles = new string[] { Roles.Admin.ToString(), Roles.User.ToString() };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                   await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // create admin user
            var admin = new IdentityUser()
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                EmailConfirmed = true
            };
            var AdminUser = await userManager.FindByEmailAsync(admin.Email);
            if (AdminUser is null) {
                await userManager.CreateAsync(admin, "Admin@123");
                await userManager.AddToRoleAsync(admin, Roles.Admin.ToString());
            }
        }
    }
}
