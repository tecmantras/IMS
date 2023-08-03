using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInManagement.Data;
using SignInManagement.Data.Model;
using System.Net;

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
                if (AuthToken.Status == false) return StatusCode(Convert.ToInt32(HttpStatusCode.Forbidden),AuthToken.Message);

                return AuthToken;
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
