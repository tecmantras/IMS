using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagememet.Data.Model;
using UserManagememet.Data.ViewModel;

namespace UserManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        public PasswordController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == model.Email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                if (token != null)
                {
                    user.UserToken = Uri.EscapeDataString(token); ;
                    await _userManager.UpdateAsync(user);

                    return new OkObjectResult(new { result = true });
                }
                else
                {
                    return new OkObjectResult(new { result = false });
                }
            }
            else
            {
                return new OkObjectResult(new { result = false });
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
