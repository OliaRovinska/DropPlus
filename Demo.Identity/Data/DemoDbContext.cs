using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Identity.Data
{
    public class DemoDbContext: IdentityDbContext<AppUser>
    {
        public DemoDbContext(DbContextOptions<DemoDbContext> options)
            : base(options) { }

        public static async System.Threading.Tasks.Task CreateAppUserAccount(IServiceProvider serviceProvider,
            IConfiguration configuration, string configPath)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                UserManager<AppUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                string username = configuration[configPath + ":Name"];
                string email = configuration[configPath + ":Email"];
                string password = configuration[configPath + ":Password"];
                string role = configuration[configPath + ":Role"];

                if (await userManager.FindByNameAsync(username) == null)
                {
                    if (await roleManager.FindByNameAsync(role) == null)
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }

                    AppUser user = new AppUser
                    {
                        UserName = username,
                        Email = email,
                        EmailConfirmed = true
                    };

                    IdentityResult result = await userManager.CreateAsync(user, password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, role);
                    }
                }
            }
        }
    }
}