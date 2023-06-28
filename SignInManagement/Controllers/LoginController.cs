using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInManagement.Data;
using SignInManagement.Data.Model;

namespace SignInManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserDetailService _userDetail;

        public LoginController(SignInManager<User> signInManager,UserDetailService userDetail)
        {
            _signInManager = signInManager;
            _userDetail = userDetail;
        }

        [HttpPost]
        public async Task<IActionResult> Login(AuthenticationRequest request)
        {
            var result = await _signInManager.PasswordSignInAsync(request.UserName,request.Password,false,false);
            if (result.Succeeded)
            {
                var user = await _userDetail.GetUserAsync(request.UserName);
                if (user == null)
                {

                }
            }
            return new OkObjectResult(result);

        }
    }
}
