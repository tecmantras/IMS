using Newtonsoft.Json;
using SignInManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SignInManagement.Data
{
    public class UserDetailService
    {



        public async Task<User> GetUserAsync(string username)
        {
            User user = new User();
            HttpClient client = new HttpClient();
            string url = "http://localhost:8003/api/Account/GetUser/";
            var response = await client.GetAsync(url + username);
            if (response.IsSuccessStatusCode)
            {
                var userresponse = await response.Content.ReadAsStringAsync();
                user = JsonConvert.DeserializeObject<User>(userresponse);

            }
            return user;

        }

    }
}
