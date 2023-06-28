using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.Arm;
using UserManagememet.Data.Interface;
using UserManagememet.Data.Model;
using UserManagememet.Data.ViewModel;
using UserManagement.Services.IRepositories;

namespace UserManagement.Services.Repositories
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Department> _departmentRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<AssignUser> _assignUserRepository;
        private readonly IRepository<IdentityRole> _roleRepository;
        private readonly IRepository<IdentityUserRole<string>> _userRoleRepository;

        public AccountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _userRepository = _unitOfWork.GetRepository<User>();
            _departmentRepository = _unitOfWork.GetRepository<Department>();
            _assignUserRepository = _unitOfWork.GetRepository<AssignUser>();
            _roleRepository = _unitOfWork.GetRepository<IdentityRole>();
            _userRoleRepository = _unitOfWork.GetRepository<IdentityUserRole<string>>();
        }
        public async Task<List<UserResponseViewModel>> GetAllUserAsync()
        {
            var users = from u in _userRepository.GetAll().Include(x => x.Department)
                        join ur in _userRoleRepository.GetAll()
                        on u.Id equals ur.UserId
                        join r in _roleRepository.GetAll()
                        on ur.RoleId equals r.Id
                        join am in _assignUserRepository.GetAll()
                        on u.Id equals am.UserId into ams
                        from am in ams.DefaultIfEmpty()
                        select new UserResponseViewModel
                        {
                            UserId = u.Id,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            Email = u.Email,
                            Phone = u.PhoneNumber,
                            Role = r.Name,
                            Department = u.Department.Name,
                            AssignedManagerId = am.AssignedManagerId,
                            AssignedHrId = am.AssignedHrId
                        };
            var result = from u in users
                         join Man in _userRepository.GetAll()
                         on u.AssignedManagerId equals Man.Id into amd
                         from Man in amd.DefaultIfEmpty()
                         join hr in _userRepository.GetAll()
                         on u.AssignedHrId equals hr.Id into hrs
                         from hr in hrs.DefaultIfEmpty()
                         select new UserResponseViewModel
                         {
                             UserId = u.UserId,
                             FirstName = u.FirstName,
                             LastName = u.LastName,
                             Email = u.Email,
                             Phone = u.Phone,
                             Role = u.Role,
                             Department = u.Department,
                             AssignManager = string.IsNullOrEmpty(Man.FirstName) ? null : Man.FirstName + " " + Man.LastName,
                             AssignHR = string.IsNullOrEmpty(hr.FirstName) ? null : hr.FirstName + " " + hr.LastName
                         };
            return await result.ToListAsync();


        }

        public async Task<UserResponseViewModel> GetByEmailUserAsync(string Email)
        {
            // var user = _userRepository.GetAll().Include(x=>x.Department).Join().FirstOrDefaultAsync(x => x.Email == Email);
            var users = await (from u in _userRepository.GetAll().Include(x => x.Department)
                        join ur in _userRoleRepository.GetAll()
                        on u.Id equals ur.UserId
                        join r in _roleRepository.GetAll()
                        on ur.RoleId equals r.Id
                        select new UserResponseViewModel
                        {
                            UserId = u.Id,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            Email = u.Email,
                            Role = r.Name,
                            Department = u.Department.Name,
                        }).Where(x=>x.Email == Email).FirstOrDefaultAsync();
            return  users;
        }
    }
}
