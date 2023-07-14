using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignInManagement.Data.Model;
using UserManagememet.Data.Interface;
using UserManagememet.Data.Model;
using UserManagememet.Data.ViewModel;
using UserManagement.Services.IRepositories;
using User = UserManagememet.Data.Model.User;

namespace UserManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailHelper _emailHelper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IRepository<AssignUser> _assignUserRepository;
        private readonly IConfiguration _configuration;

        public AccountController(IAccountService accountService, UserManager<User> userManager, IUnitOfWork unitOfWork, RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager,IEmailHelper emailHelper, IConfiguration configuration)
        {
            _accountService = accountService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _assignUserRepository = _unitOfWork.GetRepository<AssignUser>();
            _signInManager = signInManager;
            _emailHelper = emailHelper;
            _configuration = configuration;
        }
        [HttpPost("InsertRole"),Authorize(Roles ="Manager")]
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
        [HttpGet("GetAllRole"), Authorize]
        public async Task<IActionResult> GetRoleAsync()
        {
            try
            {
                var GetAllRoles = await _roleManager.Roles.ToListAsync();
                if (GetAllRoles.Any())
                {
                    return new OkObjectResult(new {succeded = true,GetAllRoles});
                }
                return new OkObjectResult(new { succeded = false});
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost("InsertUser"), Authorize(Roles = "Manager")]
        public async Task<IActionResult> CreateUserAsync([FromBody]RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new UserManagememet.Data.Model.User
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
                    EmailConfirmed = false,
                    PhoneNumberConfirmed = true,
                };
                var result = await _userManager.CreateAsync(user,model.Password);
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
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var ResetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                    if (ResetPasswordToken != null)
                    {
                        user.UserToken = token;
                        await _userManager.UpdateAsync(user);
                    }
                    var ocelotUrl = _configuration.GetValue<string>("Ocelot:BaseUrl");
                    var confimationLink = $"{ocelotUrl}/api/Account/ConfirmEmail?token={Uri.EscapeDataString(token)}&email={user.Email}";
                    var confirmPasswordLink = $"{ocelotUrl}/api/Account/xyz?token={Uri.EscapeDataString(ResetPasswordToken)}&email={user.Email}";
                    ConfimEmailRequestViewModel confimEmailModel = new ConfimEmailRequestViewModel();
                    confimEmailModel.UserEmail = user.Email;
                    confimEmailModel.ConfirmEmailLink = confimationLink;
                    confimEmailModel.ConfirmPasswordLink = confirmPasswordLink;
                    confimEmailModel.UserPassword = model.Password;
                    confimEmailModel.UserName = user.FirstName + " " + user.LastName;
                        _= await _emailHelper.VerifyEmailAsync(confimEmailModel);
                    return new OkObjectResult(new { succeded = result, model });
                }
            }
            return new OkObjectResult(new { succeeded = false });
        }
        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate(LoginViewModel model)
        {
            try
            {
                var Modeluser = await _userManager.FindByEmailAsync(model.UserName);
                if(Modeluser != null)
                {
                    if (!await _userManager.IsEmailConfirmedAsync(Modeluser))
                    {
                        return Unauthorized("Email not confirmed. Please confirm your email first.");
                    }
                }
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
                if (result.Succeeded)
                {
                    var user = await _accountService.GetByEmailUserAsync(model.UserName);
                    return new OkObjectResult(user);
                }
                return new OkObjectResult(false);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet("GetAllUser"), Authorize(Roles = "Manager")]
        
        public async Task<IActionResult> GetAllUserAsync()
        {
            try
            {
                var result = await _accountService.GetAllUserAsync();
                if (result.Any())
                {
                    return new OkObjectResult(new { Succeeded = true, result });
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
        [HttpGet("GetUser/{Email}")]
        public async Task<IActionResult> GetByEmailUser(string Email)
        {
            try
            {
                var result = await _accountService.GetByEmailUserAsync(Email);
                if (result != null)
                {
                    return new OkObjectResult(new { Succeeded = true, result });
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
        [HttpGet("GetManagerUserByManagerRoleId")]
        public async Task<IActionResult> GetUserByManagerRoleId()
        {
            try
            {
                var users = await _accountService.GetUserByManagerRoleId();
                if (users.Any())
                {
                    return new OkObjectResult(new { Succeeded = true, users });
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
        [HttpGet("User/{userId:Guid}")]
        public async Task<IActionResult> GetUserById(string UserId)
        {
            try
            {
                var user = await _accountService.GetUserByIdAsync(UserId);
                if(user != null)
                {
                    return new OkObjectResult(new { succeeded = true, user });
                }
                else
                {
                    return new OkObjectResult(new { succeeded = false });
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new OkObjectResult(new { succcesdd = false,msg="User not Found" }) ;

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return new OkObjectResult(new { succcedd = true, msg = "Email is confirmed" });
            }
            return new OkObjectResult(new { succcesdd = false, msg = result.Errors});
        }
        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUserAsync([FromBody]UpdateUserViewModel model)
        {
            try
            {
                DateTime? date = null;
                var user = await _userManager.FindByIdAsync(model.UserId);
                user.FirstName = !string.IsNullOrEmpty(model.FirstName) ? model.FirstName : user.FirstName;
                user.LastName = !string.IsNullOrEmpty(model.LastName) ? model.LastName : user.LastName;
                user.Email = !string.IsNullOrEmpty(model.Email) ? model.Email : user.Email;
                user.PhoneNumber = !string.IsNullOrEmpty(model.Phone) ? model.Phone : user.PhoneNumber;
                user.Address = !string.IsNullOrEmpty(model.Address) ? model.Address : user.Address;
                user.DOB = model.DOB.HasValue ? model.DOB.Value : user.DOB;
                user.Gender = !string.IsNullOrEmpty(model.Gender) ? model.Gender : user.Gender;
                user.DepartmentId = model.DepartmentId == 0 ? user.DepartmentId : model.DepartmentId;

                var result = await _userManager.UpdateAsync(user);
              _= _unitOfWork.commit();

                return new OkObjectResult(result.Succeeded);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost("ActiveDeactiveUser")]
        public async Task<IActionResult> UpdateActiveUser([FromBody] UpdateUserStatusViewModel model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user != null)
                {
                    user.IsActive = model.IsActive;
                    var result = await _userManager.UpdateAsync(user);
                    _ = _unitOfWork.commit();

                    return new OkObjectResult(result.Succeeded);
                }
                else
                {
                    return new OkObjectResult(false);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                return new OkObjectResult(new { succeeded =true,Msg="Password Updated Successfully" });
            }
            else
            {
                return new BadRequestObjectResult(new { succeeded = true, Msg = result.Errors });
            }
        }

        [HttpGet("IsEmailExist/{email}")]
        public async Task<IActionResult> IsEmailExist(string email)
        {
            var result = await _accountService.IsEmailExist(email);
            if (result)
            {
                return new OkObjectResult(new { succedd = result, msg = "Email already Exist" });
            }
            return new OkObjectResult(new { succedd = result});
        }

    }
}
