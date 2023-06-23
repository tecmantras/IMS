using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagememet.Data.ViewModel;

namespace UserManagement.Services.IRepositories
{
    public interface IAccountService
    {
        Task<object> GetAllUserAsync();
        Task<bool> CreateRoleAsync(RoleViewModel model);

    }
}
