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
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<AssignUser> _assignUserRepository;

        public AccountController(IAccountService accountService, UserManager<IdentityUser> userManager, IUnitOfWork unitOfWork)
        {
            _accountService = accountService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _assignUserRepository = _unitOfWork.GetRepository<AssignUser>();
        }
        [HttpPost("InsertRole")]
        public async Task<IActionResult> CreateRoleAsync(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.CreateRoleAsync(model);
                if(result == true)
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
            if(ModelState.IsValid)
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
                    return new OkObjectResult(new {succeded = result, model });
                }
            }
            return new OkObjectResult(new {succeeded = false});
        }
    }
}
