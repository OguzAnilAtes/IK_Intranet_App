using IK_Intranet_App.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace IK_Intranet_App.Data
{
    public static class DbSeeder
    {
        // Bu metod, rollerin var olup olmadığını kontrol eder, yoksa oluşturur.
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roleNames = { "Admin", "Member" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    // Rol yoksa oluştur (Admin ve Member)
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
    }
}