using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignInManagement.Data.Model
{
    public class AuthenticationResponse
    {
        public string UserName { get; set; }
        public string? Token { get; set; }
        public int ExpiresIn { get; set; }
        public string Roles { get; set; }
        public string UserId { get; set; }
        public bool Status { get;set; }
        public string Message { get; set; }
    }
}
