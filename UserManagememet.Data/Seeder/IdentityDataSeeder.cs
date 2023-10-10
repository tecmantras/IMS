using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagememet.Data.Model;

namespace UserManagememet.Data.Seeder
{
    public class IdentityDataSeeder
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IdentityDataSeeder(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            var adminRole = new IdentityRole
            {
                Id = "EE6C9626-9B0D-4226-BEC4-E46BE6B3E3E5",
                Name = "Admin",
                NormalizedName = "ADMIN"
            };
            await CreateRoleAsync(adminRole);

            var managerRole = new IdentityRole
            {
                Id = "68C5C2EB-257E-4F8B-A2D2-C1E7A36E95AD",
                Name = "Manager",
                NormalizedName = "Manager"
            };
            await CreateRoleAsync(managerRole); 
           
            var hrRole = new IdentityRole
            {
                Id = "AFF2831A-F91C-4389-8DB0-1B045B865AF2",
                Name = "HR",
                NormalizedName = "HR"
            };
            await CreateRoleAsync(hrRole); 
            
            var employeeRole = new IdentityRole
            {
                Id = "C5236014-0F74-40CB-BD05-96C664538643",
                Name = "Employee",
                NormalizedName = "Employee"
            };
            await CreateRoleAsync(employeeRole);

            var adminUserPassword = "Admin@123";
            var adminUser = new User
            {
                Id = "C5CBB7A0-BC41-4CBA-A289-BA6C1A4264D1",
                UserName = "admin@gmail.com",
                NormalizedUserName = "ADMIN@GMAIL.COM",
                Email = "admin@gmail.com",
                NormalizedEmail = "ADMIN@GMAIL.COM",
                EmailConfirmed = true,
                FirstName = "Admin",
                LastName = "Admin",
                Gender = "0",
                Address = "Address",
                CreationDate = DateTime.UtcNow
            };
            await CreateUserAsync(adminUser, adminUserPassword);

            var adminInRole = await _userManager.IsInRoleAsync(adminUser, adminRole.Name);
            if (!adminInRole)
                await _userManager.AddToRoleAsync(adminUser, adminRole.Name);
        }

        private async Task CreateRoleAsync(IdentityRole role)
        {
            var exits = await _roleManager.RoleExistsAsync(role.Name);
            if (!exits)
                await _roleManager.CreateAsync(role);
        }

        private async Task CreateUserAsync(User user, string password)
        {
            var exists = await _userManager.FindByEmailAsync(user.Email);
            if (exists == null)
                await _userManager.CreateAsync(user, password);
        }
    }
}
