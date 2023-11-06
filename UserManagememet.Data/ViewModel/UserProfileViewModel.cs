using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagememet.Data.ViewModel
{
    public class UserDetails
    {
        public UserProfileViewModel Profile { get; set; }
        public List<Listresponse> Leaves { get; set; }
    }

    public class UserProfileViewModel
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

        //public string Name { get; set; }
        //public string Email { get; set; }
        //public string Password { get; set; }
        //public string? ManagerName { get; set; }
        //public string? HRName { get; set; }
        //public string? RecentAppliedLeaves { get; set; }
        //public DateTime JoiningDate { get; set; }
        //public DateTime BirthDate { get; set; }
        //public string Gender { get; set; }
        //public string Address { get; set; }
    }

    public class LeaveList
    {
        public Data data { get; set; }
        public bool isSuccess { get; set; }
        public object message { get; set; }
    }

    public class Data
    {
        public int totalCount { get; set; }
        public List<Listresponse> listResponse { get; set; }
    }

    public class Listresponse
    {
        public int userLeaveId { get; set; }
        public object userName { get; set; }
        public object department { get; set; }
        public object role { get; set; }
        public string leaveType { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string totalDays { get; set; }
        public string reasonForLeave { get; set; }
        public int status { get; set; }
        public int statusByManager { get; set; }
        public int statusByHR { get; set; }
        public object cancellationReason { get; set; }
        public string appliedDate { get; set; }
        public object remark { get; set; }
        public DateTime fromDate { get; set; }
        public DateTime todate { get; set; }
    }

}
