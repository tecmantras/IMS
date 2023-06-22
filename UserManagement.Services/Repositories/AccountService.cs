using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Services.IRepositories;

namespace UserManagement.Services.Repositories
{
    public class AccountService : IAccountService
    {
        public Task<object> GetAllUserAsync()
        {
            throw new NotImplementedException();
        }
    }
}
