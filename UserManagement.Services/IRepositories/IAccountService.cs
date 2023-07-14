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
        Task<List<UserResponseViewModel>> GetAllUserAsync();
        Task<UserResponseViewModel?> GetUserByIdAsync(string UserId);
        Task<UserResponseViewModel> GetByEmailUserAsync(string Email);
        Task<List<UserManagerViewModel>> GetUserByManagerRoleId();
        Task<bool> IsEmailExist(string email);

    }
}
