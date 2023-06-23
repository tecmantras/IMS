using Microsoft.AspNetCore.Identity;
using UserManagememet.Data.ViewModel;
using UserManagement.Services.IRepositories;

namespace UserManagement.Services.Repositories
{
    public class AccountService : IAccountService
    {
        private readonly RoleManager<IdentityRole> _roleManager;


        public AccountService(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
            
        }


        public async Task<object> GetAllUserAsync()
        {
            return true;
        }



        public async Task<bool> CreateRoleAsync(RoleViewModel model)
        {
            IdentityRole identityRole = new IdentityRole
            {
                Name = model.RoleName
            };
            var result = await _roleManager.CreateAsync(identityRole);
            return result.Succeeded;

        }

    }
}
