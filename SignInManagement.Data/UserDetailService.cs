using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SignInManagement.Data.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static System.Net.WebRequestMethods;

namespace SignInManagement.Data
{
    public class UserDetailService
    {

        public const string JWT_SECURRITY_KEY = "ykjjssadglkWsdjg5w50i04B324Vj423432";
        public const int JWT_TOKEN_VALIDITY_MINS = 720;

        public async Task<AuthenticationResponse> Authenticate(AuthenticationRequest request)
        {
            try
            {
                AuthenticationResponse authentication = new AuthenticationResponse();
                if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
                    return null;
                UserResponseViewmodel user = new UserResponseViewmodel();
                HttpClient client = new HttpClient();
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                string url = "http://host.docker.internal:8003/api/Account/Authenticate";
                //string url = "http://localhost:5234/api/Account/Authenticate";
                var response = await client.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    var userresponse = await response.Content.ReadAsStringAsync();
                    user = JsonConvert.DeserializeObject<UserResponseViewmodel>(userresponse);
                    authentication = new AuthenticationResponse();
                    var AuthResponse = await GenerateJwtToken(user);
                    authentication.UserName = user.FirstName + " " + user.LastName;
                    authentication.Token = AuthResponse.Token;
                    authentication.ExpiresIn = AuthResponse.ExpiresIn;
                    authentication.Roles = user.Role;
                    authentication.Status = user.Status;
                    authentication.UserId = user.UserId;


                }
                var errorResponse = await response.Content.ReadAsStringAsync();
               var error = JsonConvert.DeserializeObject<ErrorResponseViewModel>(errorResponse);
                authentication.Status = error.Status;
                authentication.Message = error.Msg;
                return authentication;
            }
            catch (Exception)
            {

                throw;
            }

        }
        public async Task<AuthenticationResponse> GenerateJwtToken(UserResponseViewmodel user)
        {
            try
            {
                var userFullName = user.FirstName + " " + user.LastName;
                var claims = new List<Claim>{

                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim("Username", user.Email),
                new Claim("userFullName", userFullName),
                new Claim(ClaimTypes.NameIdentifier,user.UserId),
                new Claim("UserId",user.UserId),
                new Claim(ClaimTypes.Role, user.Role)
            };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWT_SECURRITY_KEY));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
                var expires = DateTime.Now.AddMinutes(JWT_TOKEN_VALIDITY_MINS);
                var token = new JwtSecurityToken(
                            claims: claims,
                            expires: expires,
                            signingCredentials: creds
                        );
                AuthenticationResponse responseViewmodel = new AuthenticationResponse
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    ExpiresIn = (int)expires.Subtract(DateTime.Now).TotalSeconds,
                };
                return responseViewmodel;
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
