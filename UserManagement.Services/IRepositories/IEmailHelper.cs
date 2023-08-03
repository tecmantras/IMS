using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagememet.Data.ViewModel;

namespace UserManagement.Services.IRepositories
{
    public interface IEmailHelper
    {
        Task<bool> VerifyEmailAsync(ConfimEmailRequestViewModel model);
        Task<bool> ForgotPasswordEmailAsync(ForgotPasswordEmailViewModel model);
    }
}
