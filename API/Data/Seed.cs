using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using API.data;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager)
        {

            if (await roleManager.Roles.AnyAsync() == false)
            {
                var roles = new List<AppRole> {
                    new AppRole{ Name = "Member" },
                    new AppRole{ Name = "Admin" },
                    new AppRole{ Name = "Moderator" }
                };

                foreach (var role in roles)
                {
                    await roleManager.CreateAsync(role);
                }

                var userData = await System.IO.File.ReadAllTextAsync("Data/UserDataSeed.json");
                var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

                foreach (var user in users)
                {
                    //using var hmac = new HMACSHA512();
                    // user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("welcome1"));
                    // user.PasswordKey = hmac.Key;
                    //context.Users.Add(user);
                    user.UserName = user.UserName.ToLower();
                    await userManager.CreateAsync(user, "Welcome@123");
                    await userManager.AddToRoleAsync(user, "Member");
                }

                var admin = new AppUser { UserName = "admin" };
                await userManager.CreateAsync(admin, "Welcome@123");
                await userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });
            }


        }
    }
}