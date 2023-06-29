using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SignInManagement.Data.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SignInManagement.Data
{
    public class UserDetailService
    {

        public const string JWT_SECURRITY_KEY = "ykjjssadglkWsdjg5w50i04B324Vj423432";
        public const int JWT_TOKEN_VALIDITY_MINS = 20;

        public async Task<AuthenticationResponse> Authenticate(AuthenticationRequest request)
        {
            try
            {
                AuthenticationResponse authentication = new AuthenticationResponse();
                if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
                    return null;
                User user = new User();
                HttpClient client = new HttpClient();
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                string url = "http://host.docker.internal:8003/api/Account/Authenticate";
                var response = await client.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    var userresponse = await response.Content.ReadAsStringAsync();
                    if(userresponse== "false")
                    {
                        return null;
                    }
                    user = JsonConvert.DeserializeObject<User>(userresponse);
                    var token = await GenerateJwtToken(user);
                    authentication.UserName = user.FirstName + " " + user.LastName;
                    authentication.Token = token;

                }
                return authentication;
            }
            catch (Exception)
            {

                throw;
            }

        }
        public async Task<string> GenerateJwtToken(User user)
        {
            try
            {

                var claims = new List<Claim>{

                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim("Username", user.Email),
                new Claim(ClaimTypes.NameIdentifier,user.UserId),
                new Claim("Role", user.Role)
            };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWT_SECURRITY_KEY));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
                var expires = DateTime.Now.AddMinutes(JWT_TOKEN_VALIDITY_MINS);
                var token = new JwtSecurityToken(
                            claims: claims,
                            expires: expires,
                            signingCredentials: creds
                        );
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
