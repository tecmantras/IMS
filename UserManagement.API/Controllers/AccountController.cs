using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserManagememet.Data.Interface;
using UserManagememet.Data.Model;
using UserManagememet.Data.ViewModel;
using UserManagement.Services.IRepositories;

namespace UserManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IRepository<AssignUser> _assignUserRepository;

        public AccountController(IAccountService accountService, UserManager<User> userManager, IUnitOfWork unitOfWork, RoleManager<IdentityRole> roleManager)
        {
            _accountService = accountService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _assignUserRepository = _unitOfWork.GetRepository<AssignUser>();
        }
        [HttpPost("InsertRole")]
        public async Task<IActionResult> CreateRoleAsync(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                // var result = await _accountService.CreateRoleAsync(model);
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };
                var result = await _roleManager.CreateAsync(identityRole);
                if (result.Succeeded == true)
                {
                    return new OkObjectResult(new { succeded = true, model });
                }
                else
                {
                    return new OkObjectResult(new { succeded = false });
                }
            }
            else
            {
                return new OkObjectResult(new { succeded = false });
            }
        }

        [HttpPost("InsertUser")]
        public async Task<IActionResult> CreateUserAsync(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.Phone,
                    Address = model.Address,
                    Gender = model.Gender,
                    JoiningDate = model.JoiningDate,
                    DOB = model.DOB,
                    DepartmentId = model.DepartmentId,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded == true)
                {
                    _ = await _userManager.AddToRoleAsync(user, model.Role);
                    if (model.AssignedHrId != null || model.AssignedManagerId != null)
                    {
                        var assignUser = new AssignUser
                        {
                            UserId = user.Id,
                            AssignedHrId = model.AssignedHrId,
                            AssignedManagerId = model.AssignedManagerId
                        };
                        _ = _assignUserRepository.Add(assignUser);
                        var assignResult = _unitOfWork.commit();

                    }
                    return new OkObjectResult(new { succeded = result, model });
                }
            }
            return new OkObjectResult(new { succeeded = false });
        }

        [HttpGet("GetAllUser")]
        public async Task<IActionResult> GetAllUserAsync()
        {
            try
            {
                var result = await _accountService.GetAllUserAsync();
                if (result.Any())
                {
                    return new OkObjectResult(new { Succeeded = true ,result}) ;
                }
                else
                {
                    return new OkObjectResult(new { Succeeded = false });
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
