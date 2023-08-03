using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagememet.Data.ViewModel
{
    public class UpdateUserViewModel
    {
        public string UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public int? DepartmentId { get; set; }
        public DateTime? DOB { get; set; }
        public string? Role { get; set; }
        public DateTime JoiningDate { get; set; }
        public string? AssignedManagerId { get; set; }
        public string? AssignedHrId { get; set; }
    }
}
