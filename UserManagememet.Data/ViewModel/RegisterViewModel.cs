using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagememet.Data.ViewModel
{
    public class RegisterViewModel
    {
        [Required]
        [MaxLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        public string Phone { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        [Required]
        public string Role { get; set; }
        public DateTime DOB { get; set; }
        public DateTime JoiningDate { get; set; }
        public int? DepartmentId { get; set; }
        public string? AssignedManagerId { get; set; }
        public string? AssignedHrId { get; set; }
    }

    public class UserViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public int DepartmentId { get; set; }
        public DateTime DOB { get; set; }

    }
    public class UserResponseViewModel
    {
        public string? UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? AssignManager { get; set; }
        public string? AssignHR { get; set; }
        public string? Department { get; set; }
        public string? Role { get; set; }
        public string? AssignedManagerId { get; set; }
        public string? AssignedHrId { get; set; }

    }
    public class LoginViewModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

}
