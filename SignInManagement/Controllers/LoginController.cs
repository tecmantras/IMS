using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
        private readonly UserDetailService _userDetail;

        public LoginController(UserDetailService userDetail)
        {
            _userDetail = userDetail;
        }

        [HttpPost]
        public  ActionResult<AuthenticationResponse> Authenticate(AuthenticationRequest request)
        {
            try
            {
                var AuthToken =  _userDetail.Authenticate(request).Result;
                if (AuthToken == null) return Unauthorized();

                return AuthToken;
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
