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
        public bool? IsActive { get; set; }
        public string? Address { get; set; }
        public string? DOB { get; set; }
        public string? DOJ { get; set; }
        public int? DepartmentId { get; set; }
        public string? Gender { get; set; }
        public bool Status { get; set; }
        public string? AssignManagerEmail { get; set; }
        public string? AssignHREmail { get; set; }
        public int? TotalUsers { get; set; }
    }
    public class LoginViewModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public class UserManagerViewModel
    {
        public string? UserId { get; set; }
        public string Name { get; set; }
        public string RoleId { get; set; }
    }
    public class UserLeaveBalanceViewModel
    {
        public string? UserId { get; set; }
    }
    public class ChangePasswordViewModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string NewPassword { get; set; }
    }
    public class ForgotPasswordViewModel
    {
        [Required]
        public string Email { get; set; }
       
    }
    public class ResetPasswordViewModel
    {
        [Required]
        public string token { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
    public class ForgotPasswordEmailViewModel
    {
        public string Email { get; set; }
        public string confirmPasswordLink { get; set; }
        public string  UserName { get; set; }

    }
    public class UserHRViewModel
    {
        public string? UserId { get; set; }
        public string Name { get; set; }
    }

    public class PagedListUserViewModel
    {
        public int TotalCount { get; set; }
        public List<UserResponseViewModel> userResponses { get; set; }
    }
}
