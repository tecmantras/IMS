using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignInManagement.Data.Model
{
    public class UserResponseViewmodel
    {
        public string? UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
       // public string Gender { get; set; }
      //  public DateTime DOB { get; set; }
        public string? Role { get; set; }
    }
}
