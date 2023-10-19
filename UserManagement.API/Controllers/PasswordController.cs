using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net;
using UserManagememet.Data.Model;
using UserManagememet.Data.ViewModel;
using UserManagement.Services.IRepositories;

namespace UserManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailHelper _emailHelper;
        private readonly IConfiguration _configuration;

        public PasswordController(UserManager<User> userManager, IEmailHelper emailHelper, IConfiguration configuration)
        {
            _userManager = userManager;
            _emailHelper = emailHelper;
            _configuration = configuration;
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            try
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == model.Email);
                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    if (token != null)
                    {
                        user.UserToken = Uri.EscapeDataString(token);
                        await _userManager.UpdateAsync(user);
                        var frontEnd = _configuration.GetValue<string>("FrontEnd:BaseUrl");
                        var endPoint = _configuration.GetValue<string>("FrontEnd:EndPoint");
                        var confirmPasswordLink = $"{frontEnd}/{endPoint}?token={Uri.EscapeDataString(token)}&email={user.Email}";
                        ForgotPasswordEmailViewModel forgotpswrd = new ForgotPasswordEmailViewModel
                        {
                            UserName = user.FirstName + " " + user.LastName,
                            Email = user.Email,
                            confirmPasswordLink = confirmPasswordLink
                        };
                        var result = await _emailHelper.ForgotPasswordEmailAsync(forgotpswrd);
                        if (result)
                        {
                            return new OkObjectResult(new { result = true, msg = "Reset Password Mail Sent" });
                        }
                        else
                        {
                            return new OkObjectResult(new { result = false, msg = "Something went wrong" });
                        }
                    }
                    else
                    {
                        return new OkObjectResult(new { result = false, msg = "User not found" });
                    }
                }
                else
                {
                    return new OkObjectResult(new { result = false });
                }
            }
            catch (Exception ex)
            {

                return new OkObjectResult(new { result = false, msg = ex.Message });
            }
        }
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserToken == model.token);
            if (user != null)
            {
                var token = Uri.UnescapeDataString(user.UserToken);
                var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                if (result.Succeeded)
                {
                    user.UserToken = null;
                    await _userManager.UpdateAsync(user);
                    return new OkObjectResult(result.Succeeded);

                }
                return new OkObjectResult(result);

            }
            else
            {
                return new NotFoundResult();
            }
        }

    }
}
