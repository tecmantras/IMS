using Azure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.Arm;
using UserManagememet.Data.Constant;
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
        public async Task<PagedListUserViewModel> GetAllUserAsync(int Page, int PageSize = 10, string? SearchValue = null)
        {
            PagedListUserViewModel pagedList = new PagedListUserViewModel();




            var users = from u in _userRepository.GetAll().Include(x => x.Department)
                        join ur in _userRoleRepository.GetAll()
                        on u.Id equals ur.UserId
                        join r in _roleRepository.GetAll()
                        on ur.RoleId equals r.Id
                        join am in _assignUserRepository.GetAll()
                        on u.Id equals am.UserId into ams
                        from am in ams.DefaultIfEmpty()
                        where !u.IsDeleted && (SearchValue == null || (
                              (SearchValue != null && u.FirstName.Contains(SearchValue)) ||
                              (SearchValue != null && u.LastName.Contains(SearchValue)) ||
                              (SearchValue != null && u.Email.Contains(SearchValue)) ||
                              (SearchValue != null && r.Name.Contains(SearchValue)) ||
                              (SearchValue != null && u.Department.Name.Contains(SearchValue)
                              )))
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
                            AssignedHrId = am.AssignedHrId,
                            IsActive = u.IsActive,
                            Address = u.Address,
                            DOB = u.DOB.ToString(ConstantData.DateFormat),
                            DOJ = u.JoiningDate.ToString(ConstantData.DateFormat),
                            DepartmentId = u.DepartmentId,
                            Gender = u.Gender
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
                             AssignHR = string.IsNullOrEmpty(hr.FirstName) ? null : hr.FirstName + " " + hr.LastName,
                             IsActive = u.IsActive,
                             Address = u.Address,
                             DOB = u.DOB,
                             DOJ = u.DOJ,
                             DepartmentId = u.DepartmentId,
                             AssignedManagerId = u.AssignedManagerId,
                             AssignedHrId = u.AssignedHrId,
                             Gender = u.Gender
                         };
            pagedList.userResponses = await result.Skip((Page - 1) * PageSize)
             .Take(PageSize)
             .ToListAsync();
            pagedList.TotalCount = result.Count();
            return pagedList;
        }

        public async Task<UserResponseViewModel> GetByEmailUserAsync(string Email)
        {
            // var user = _userRepository.GetAll().Include(x=>x.Department).Join().FirstOrDefaultAsync(x => x.Email == Email);
            var users = await (from u in _userRepository.GetAll().Include(x => x.Department)
                               join ur in _userRoleRepository.GetAll()
                               on u.Id equals ur.UserId
                               join r in _roleRepository.GetAll()
                               on ur.RoleId equals r.Id
                               where u.Email == Email
                               select new UserResponseViewModel
                               {
                                   UserId = u.Id,
                                   FirstName = u.FirstName,
                                   LastName = u.LastName,
                                   Email = u.Email,
                                   Role = r.Name,
                                   Department = u.Department.Name,
                                   Status = true,
                               }).FirstOrDefaultAsync();
            return users;
        }

        public async Task<UserResponseViewModel?> GetUserByIdAsync(string UserId)
        {
            try
            {
                var users = from u in _userRepository.GetAll().Include(x => x.Department)
                            join ur in _userRoleRepository.GetAll()
                            on u.Id equals ur.UserId
                            join r in _roleRepository.GetAll()
                            on ur.RoleId equals r.Id
                            join am in _assignUserRepository.GetAll()
                            on u.Id equals am.UserId into ams
                            from am in ams.DefaultIfEmpty()
                            where u.Id == UserId && !u.IsDeleted
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
                var result = await (from u in users
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
                                        AssignHR = string.IsNullOrEmpty(hr.FirstName) ? null : hr.FirstName + " " + hr.LastName,
                                        AssignedManagerId = u.AssignedManagerId,
                                        AssignedHrId = u.AssignedHrId,
                                        AssignManagerEmail = Man.Email,
                                        AssignHREmail = hr.Email
                                    }).FirstOrDefaultAsync();
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<UserManagerViewModel>> GetUserByManagerRoleId()
        {
            try
            {
                var users = await (from u in _userRepository.GetAll()
                                   join ur in _userRoleRepository.GetAll()
                                   on u.Id equals ur.UserId
                                   join r in _roleRepository.GetAll()
                                   on ur.RoleId equals r.Id
                                   where r.Name == UserRole.Manager
                                   select new UserManagerViewModel
                                   {
                                       UserId = u.Id,
                                       Name = u.FirstName + " " + u.LastName
                                   }).ToListAsync();
                return users;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> IsEmailExist(string email)
        {
            try
            {
                var userCount = await _userRepository.GetAll().Where(x => x.Email == email && !x.IsDeleted).CountAsync();
                if (userCount > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<UserHRViewModel>> GetUserByHRRoleId()
        {
            try
            {
                var users = await (from u in _userRepository.GetAll()
                                   join ur in _userRoleRepository.GetAll()
                                   on u.Id equals ur.UserId
                                   join r in _roleRepository.GetAll()
                                   on ur.RoleId equals r.Id
                                   where r.Name == UserRole.HR
                                   select new UserHRViewModel
                                   {
                                       UserId = u.Id,
                                       Name = u.FirstName + " " + u.LastName,
                                   }).ToListAsync();
                return users;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<PagedListUserViewModel> GetUserByManagerOrHRIdAsync(string userId, int page, int pageSize = 10, string? searchValue = null)
        {
            try
            {
                PagedListUserViewModel pagedList = new PagedListUserViewModel();
                var users = from u in _userRepository.GetAll().Include(x => x.Department)
                            join ur in _userRoleRepository.GetAll()
                            on u.Id equals ur.UserId
                            join r in _roleRepository.GetAll()
                            on ur.RoleId equals r.Id
                            join am in _assignUserRepository.GetAll()
                            on u.Id equals am.UserId into ams
                            from am in ams.DefaultIfEmpty()
                            where am.AssignedHrId == userId || am.AssignedManagerId == userId && u.IsActive &&
                             (searchValue == null || (
                              (searchValue != null && u.FirstName.Contains(searchValue)) ||
                              (searchValue != null && u.LastName.Contains(searchValue)) ||
                              (searchValue != null && u.Email.Contains(searchValue)) ||
                              (searchValue != null && r.Name.Contains(searchValue)) ||
                              (searchValue != null && u.Department.Name.Contains(searchValue)
                              )))
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
                                AssignedHrId = am.AssignedHrId,
                                DOJ = u.JoiningDate.ToString(ConstantData.DateFormat)
                            };
                var result = await (from u in users
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
                                        AssignHR = string.IsNullOrEmpty(hr.FirstName) ? null : hr.FirstName + " " + hr.LastName,
                                        AssignedManagerId = u.AssignedManagerId,
                                        AssignedHrId = u.AssignedHrId,
                                        DOJ = u.DOJ
                                    }).ToListAsync();
                
                pagedList.userResponses = result.Skip((page - 1) * pageSize)
             .Take(pageSize)
             .ToList();
                pagedList.TotalCount = result.Count();
                return pagedList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<UserResponseViewModel?>> GetUserByDepartmentIdAsync(int departmentId)
        {
            var user = await _userRepository.GetAll().Include(x => x.Department).
                 Where(x => x.DepartmentId == departmentId).Select(u => new UserResponseViewModel
                 {
                     UserId = u.Id,
                     FirstName = u.FirstName,
                     LastName = u.LastName,
                 }).ToListAsync();
            return user;
        }
    }
}
