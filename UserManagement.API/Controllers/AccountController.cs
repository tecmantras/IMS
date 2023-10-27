using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SignInManagement.Data.Model;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Text;
using UserManagememet.Data.Constant;
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

        public AccountController(IAccountService accountService, UserManager<User> userManager, IUnitOfWork unitOfWork, RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager, IEmailHelper emailHelper, IConfiguration configuration)
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

        [HttpPost("InsertRole")]
        public async Task<IActionResult> CreateRoleAsync(RoleViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // var result = await _accountService.CreateRoleAsync(model);
                    IdentityRole identityRole = new IdentityRole
                    {
                        Name = model.RoleName
                    };
                    var result = await _roleManager.CreateAsync(identityRole);
                    if (result.Succeeded)
                    {

                        return new OkObjectResult(new ResponseMessageViewModel
                        {
                            IsSuccess = result.Succeeded,
                            Data = model
                        });
                    }
                    else
                    {
                        return new OkObjectResult(new ResponseMessageViewModel
                        {
                            IsSuccess = result.Succeeded,
                            Data = model
                        });
                    }
                }
                var response = new ResponseMessageViewModel
                {
                    IsSuccess = false,
                    Data = model,
                    Message = "Model is not validate"
                };
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return new OkObjectResult(new ResponseMessageViewModel
                {
                    IsSuccess = false,
                    Message = ex.Message,
                });
            }
        }
        [HttpGet("GetAllRole")]
        public async Task<IActionResult> GetRoleAsync()
        {
            try
            {
                var GetAllRoles = await _roleManager.Roles.ToListAsync();
                if (GetAllRoles.Any())
                {
                    //return new OkObjectResult(new { succeded = true, GetAllRoles });
                    return new OkObjectResult(new ResponseMessageViewModel
                    {
                        IsSuccess = true,
                        Data = GetAllRoles
                    });
                }
                return new OkObjectResult(new ResponseMessageViewModel
                {
                    IsSuccess = false,
                    Data = GetAllRoles
                });
            }
            catch (Exception ex)
            {
                return new OkObjectResult(new ResponseMessageViewModel
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPost("InsertUser"), Authorize(Roles = "HR,Admin,Manager")]
        public async Task<IActionResult> CreateUserAsync([FromBody] RegisterViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.Role == UserRole.Employee && (string.IsNullOrEmpty(model.AssignedHrId) || string.IsNullOrEmpty(model.AssignedManagerId)))
                    {
                        // return new BadRequestObjectResult(new { succeeded = false, msg = "Assign ManagerID or Assign HRID not be null" });
                        return new OkObjectResult(new ResponseMessageViewModel
                        {
                            IsSuccess = false,
                            Message = "Assign ManagerID or Assign HRID not be null"
                        });
                    }
                    else
                    {
                        var mailResult = await _accountService.IsEmailExist(model.Email);
                        if (!mailResult)
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
                            var result = await _userManager.CreateAsync(user, model.Password);
                            if (result.Succeeded == true)
                            {
                                var roles = await _userManager.AddToRoleAsync(user, model.Role);
                                if (roles.Succeeded)
                                {
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
                                    HttpClient client = new HttpClient();
                                    var url = $"http://host.docker.internal:8005/api/UserLeaveBalance";
                                    UserLeaveBalanceViewModel viewModel = new UserLeaveBalanceViewModel
                                    {
                                        UserId = user.Id
                                    };

                                    var jsonData = JsonConvert.SerializeObject(viewModel);
                                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                                    var response = await client.PostAsync(url, content);
                                    if (ResetPasswordToken != null)
                                    {
                                        user.UserToken = Uri.EscapeDataString(ResetPasswordToken);
                                        await _userManager.UpdateAsync(user);
                                    }
                                    var ocelotUrl = _configuration.GetValue<string>("Ocelot:BaseUrl");
                                    var frontEnd = _configuration.GetValue<string>("FrontEnd:BaseUrl");
                                    var endPoint = _configuration.GetValue<string>("FrontEnd:EndPoint");
                                    var confimationLink = $"{ocelotUrl}/api/Account/ConfirmEmail?token={Uri.EscapeDataString(token)}&email={user.Email}";
                                    var confirmPasswordLink = $"{frontEnd}/{endPoint}?token={Uri.EscapeDataString(ResetPasswordToken)}&email={user.Email}";
                                    ConfimEmailRequestViewModel confimEmailModel = new ConfimEmailRequestViewModel();
                                    confimEmailModel.UserEmail = user.Email;
                                    confimEmailModel.ConfirmEmailLink = confimationLink;
                                    confimEmailModel.ConfirmPasswordLink = confirmPasswordLink;
                                    confimEmailModel.UserPassword = model.Password;
                                    confimEmailModel.UserName = user.FirstName + " " + user.LastName;
                                    _ = await _emailHelper.VerifyEmailAsync(confimEmailModel);

                                    // return new OkObjectResult(new { succeded = result, model });
                                    return new OkObjectResult(new ResponseMessageViewModel
                                    {
                                        IsSuccess = true,
                                        Data = model,
                                        Message = "User Added"
                                    });
                                }
                                else
                                {
                                    return new OkObjectResult(new ResponseMessageViewModel
                                    {
                                        IsSuccess = false,
                                        Message = result.Errors.Select(x => x.Description).FirstOrDefault()
                                    }); ; ;
                                }

                            }
                            else
                            {
                                return new OkObjectResult(new ResponseMessageViewModel
                                {
                                    IsSuccess = false,
                                    Message = result.Errors.Select(x => x.Description).FirstOrDefault()
                                }); ;
                            }
                        }
                        else
                        {
                            return new OkObjectResult(new ResponseMessageViewModel
                            {
                                IsSuccess = false,
                                Message = "Email already exist"
                            });
                            // return new BadRequestObjectResult(new { succeeded = false, msg = "Email already exist" });
                        }
                    }
                }
                return new OkObjectResult(new ResponseMessageViewModel
                {
                    IsSuccess = false,
                    Message = "Model is not validate"
                });
            }
            catch (Exception ex)
            {

                return new OkObjectResult(new ResponseMessageViewModel
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
        }
        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate(LoginViewModel model)
        {
            try
            {
                var Modeluser = await _userManager.FindByEmailAsync(model.UserName);
                if (Modeluser != null)
                {
                    if (Modeluser.IsActive && !Modeluser.IsDeleted)
                    {
                        if (!await _userManager.IsEmailConfirmedAsync(Modeluser))
                        {
                            return new BadRequestObjectResult(new { status = false, msg = "Email not confirmed. Please confirm your email first." });
                        }
                        var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
                        if (result.Succeeded)
                        {
                            var user = await _accountService.GetByEmailUserAsync(model.UserName);
                            return new OkObjectResult(user);
                        }
                        else
                        {
                            return new BadRequestObjectResult(new { status = false, msg = "Incorrect username or password." });
                        }
                    }
                    else
                    {
                        return new BadRequestObjectResult(new { status = false, msg = "Your account is deactivated. Please contact your Adminastator." });
                    }
                }

                return new BadRequestObjectResult(new { status = false, msg = "Incorrect username or password" });
            }
            catch (Exception ex)
            {

                return new BadRequestObjectResult(new { succeeded = false, msg = ex.Message });
            }
        }

        [HttpGet("GetAllUser"), Authorize(Roles = "HR,Admin,Manager,Employee")]
        public async Task<IActionResult> GetAllUserAsync([FromQuery] int Page, [FromQuery] int PageSize = 10, [FromQuery] string? SearchValue = null)
        {
            try
            {
                var result = await _accountService.GetAllUserAsync(Page, PageSize, SearchValue);
                if (result != null)
                {
                    return new OkObjectResult(new ResponseMessageViewModel
                    {
                        IsSuccess = true,
                        Data = result
                    });
                }
                else
                {
                    return new OkObjectResult(new ResponseMessageViewModel
                    {
                        IsSuccess = false,
                        Message = "User list not found"
                    });
                }
            }
            catch (Exception ex)
            {
                return new OkObjectResult(new ResponseMessageViewModel
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
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
                    return new OkObjectResult(new ResponseMessageViewModel
                    {
                        IsSuccess = true,
                        Data = result
                    });
                }
                else
                {
                    return new OkObjectResult(new ResponseMessageViewModel
                    {
                        IsSuccess = false,
                        Message = "User not found"
                    });
                }
            }
            catch (Exception ex)
            {
                return new OkObjectResult(new ResponseMessageViewModel
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
        }
        [HttpGet("GetManagerUserByManagerRoleId"), Authorize(Roles = "HR,Admin,Manager")]
        public async Task<IActionResult> GetUserByManagerRoleId()
        {
            try
            {
                var users = await _accountService.GetUserByManagerRoleId();
                if (users.Any())
                {
                    //return new OkObjectResult(new { Succeeded = true, users });
                    return new OkObjectResult(new ResponseMessageViewModel
                    {
                        IsSuccess = true,
                        Data = users
                    });
                }
                else
                {
                    return new OkObjectResult(new ResponseMessageViewModel
                    {
                        IsSuccess = false,
                        Message = "Manager list not found"
                    });
                }

            }
            catch (Exception ex)
            {
                return new OkObjectResult(new ResponseMessageViewModel
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
        }
        [HttpGet("User/{userId}"), Authorize(Roles = "HR,Admin,Manager,Employee")]
        public async Task<IActionResult> GetUserById(string UserId)
        {
            try
            {
                var user = await _accountService.GetUserByIdAsync(UserId);
                if (user != null)
                {
                    return new OkObjectResult(new ResponseMessageViewModel
                    {
                        IsSuccess = true,
                        Data = user
                    });
                }
                else
                {
                    return new OkObjectResult(new ResponseMessageViewModel
                    {
                        IsSuccess = false,
                        Message = "User not found"
                    });
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
                return new OkObjectResult(new ResponseMessageViewModel
                {
                    IsSuccess = false,
                    Message = "User not found"
                });

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return new OkObjectResult(new ResponseMessageViewModel
                {
                    IsSuccess = true,
                    Message = "Email is confirmed"
                });

            }
            return new OkObjectResult(new ResponseMessageViewModel
            {
                IsSuccess = true,
                Message = result.Errors.ToString()
            });
        }

        [HttpPut("UpdateUser"), Authorize(Roles = "HR,Admin,Manager")]
        public async Task<IActionResult> UpdateUserAsync([FromBody] UpdateUserViewModel model)
        {
            try
            {

                DateTime? date = null;
                var user = await _userManager.FindByIdAsync(model.UserId);
                var email = user.Email;

                if (user != null)
                {
                    user.FirstName = !string.IsNullOrEmpty(model.FirstName) ? model.FirstName : user.FirstName;
                    user.LastName = !string.IsNullOrEmpty(model.LastName) ? model.LastName : user.LastName;
                    user.Email = !string.IsNullOrEmpty(model.Email) ? model.Email : user.Email;
                    user.PhoneNumber = !string.IsNullOrEmpty(model.Phone) ? model.Phone : user.PhoneNumber;
                    user.Address = !string.IsNullOrEmpty(model.Address) ? model.Address : user.Address;
                    user.DOB = model.DOB.HasValue ? model.DOB.Value : user.DOB;
                    user.Gender = !string.IsNullOrEmpty(model.Gender) ? model.Gender : user.Gender;
                    user.DepartmentId = model.DepartmentId == null ? user.DepartmentId : model.DepartmentId;

                    if (email != user.Email)
                    {
                        var mailResult = await _accountService.IsEmailExist(model.Email);
                        if (mailResult)
                        {
                            return new OkObjectResult(new ResponseMessageViewModel
                            {
                                IsSuccess = false,
                                Message = "Email already exist"
                            });
                        }
                        else
                        {
                            user.EmailConfirmed = false;
                            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                            var ocelotUrl = _configuration.GetValue<string>("Ocelot:BaseUrl");
                            var frontEnd = _configuration.GetValue<string>("FrontEnd:BaseUrl");
                            var endPoint = _configuration.GetValue<string>("FrontEnd:EndPoint");
                            var confimationLink = $"{ocelotUrl}/api/Account/ConfirmEmail?token={Uri.EscapeDataString(token)}&email={user.Email}";
                            ConfimEmailRequestViewModel confimEmailModel = new ConfimEmailRequestViewModel();
                            confimEmailModel.UserEmail = user.Email;
                            confimEmailModel.ConfirmEmailLink = confimationLink;
                            confimEmailModel.UserName = user.FirstName + " " + user.LastName;
                            _ = await _emailHelper.VerifyEmailAsync(confimEmailModel);
                        }

                    }
                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        if (model.Role != null)
                        {
                            var currentRoles = await _userManager.GetRolesAsync(user);
                            await _userManager.RemoveFromRolesAsync(user, currentRoles);
                            await _userManager.AddToRoleAsync(user, model.Role);
                        }

                        var assignUser = await _assignUserRepository.GetAll().Where(x => x.UserId == model.UserId).FirstOrDefaultAsync();
                        if (assignUser != null)
                        {
                            assignUser.AssignedHrId = !string.IsNullOrEmpty(model.AssignedHrId) ? model.AssignedHrId : assignUser.AssignedHrId;
                            assignUser.AssignedManagerId = !string.IsNullOrEmpty(model.AssignedManagerId) ? model.AssignedManagerId : assignUser.AssignedManagerId;
                            _ = _assignUserRepository.Update(assignUser);

                        }
                        else
                        {
                            var assignedUser = new AssignUser
                            {
                                UserId = user.Id,
                                AssignedHrId = model.AssignedHrId,
                                AssignedManagerId = model.AssignedManagerId
                            };
                            _ = _assignUserRepository.Add(assignedUser);
                        }
                        _ = _unitOfWork.commit();
                        return new OkObjectResult(new ResponseMessageViewModel
                        {
                            IsSuccess = true,
                            Message = "User Updated"
                        }); ;
                    }
                    else
                    {
                        return new OkObjectResult(new ResponseMessageViewModel
                        {
                            IsSuccess = false,
                            Message = result.Errors.ToString()
                        }); ;
                    }

                }
                else
                {
                    return new OkObjectResult(new ResponseMessageViewModel
                    {
                        IsSuccess = false,
                        Message = "User not found"
                    });
                }
            }
            catch (Exception ex)
            {

                return new BadRequestObjectResult(new { succeded = false, msg = ex.Message });
            }
        }

        [HttpPost("ActiveDeactiveUser"), Authorize(Roles = "HR,Admin,Manager")]
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
                    if (result.Succeeded)
                    {
                        if (model.IsActive)
                        {
                            return new OkObjectResult(new ResponseMessageViewModel
                            {
                                IsSuccess = true,
                                Message = "User is Activeted"
                            });
                            //   return new OkObjectResult(new { succeeded = result.Succeeded, Msg = "User is Activeted" });
                        }
                        else
                        {
                            return new OkObjectResult(new ResponseMessageViewModel
                            {
                                IsSuccess = true,
                                Message = "User is Deactiveted"
                            });
                        }
                    }
                    else
                    {
                        return new OkObjectResult(new ResponseMessageViewModel
                        {
                            IsSuccess = true,
                            Message = result.Errors.ToString()
                        });
                    }
                }
                else
                {
                    return new OkObjectResult(new ResponseMessageViewModel
                    {
                        IsSuccess = true,
                        Message = "User is not found"
                    }); ;
                }
            }
            catch (Exception ex)
            {

                return new OkObjectResult(new ResponseMessageViewModel
                {
                    IsSuccess = true,
                    Message = ex.Message
                });
            }
        }


        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            try
            {

                var user = await _userManager.FindByEmailAsync(model.Email);
                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    return new OkObjectResult(new ResponseMessageViewModel
                    {
                        IsSuccess = true,
                        Message = "Password Updated Successfully"
                    });
                    // return new OkObjectResult(new { succeeded = true, Msg = "Password Updated Successfully" });
                }
                else
                {
                    return new OkObjectResult(new ResponseMessageViewModel
                    {
                        IsSuccess = false,
                        Message = result.Errors.ToString()
                    });
                }
            }
            catch (Exception ex)
            {

                return new OkObjectResult(new ResponseMessageViewModel
                {
                    IsSuccess = true,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Check that the given email is already exists or not
        /// </summary>
        /// <param name="email"></param>
        /// <returns>If email already exists then true otherwise false</returns>
        [HttpGet("IsEmailExist/{email}")]
        public async Task<IActionResult> IsEmailExist(string email)
        {
            var result = await _accountService.IsEmailExist(email);
            if (result)
            {
                return new OkObjectResult(new ResponseMessageViewModel
                {
                    IsSuccess = result,
                    Message = "Email already Exist"
                });

            }
            return new OkObjectResult(new ResponseMessageViewModel
            {
                IsSuccess = result,
            });
        }
        [HttpGet("GetUserByHRRoleId"), Authorize(Roles = "HR,Admin,Manager")]
        public async Task<IActionResult> GetUserByHRRoleId()
        {
            try
            {
                var users = await _accountService.GetUserByHRRoleId();
                if (users.Any())
                {
                    return new OkObjectResult(new ResponseMessageViewModel
                    {
                        IsSuccess = true,
                        Data = users
                    });
                    //return new OkObjectResult(new { Succeeded = true, users });
                }
                else
                {
                    return new OkObjectResult(new ResponseMessageViewModel
                    {
                        IsSuccess = false,
                        Message = "User list not found"
                    });
                }

            }
            catch (Exception ex)
            {

                return new OkObjectResult(new ResponseMessageViewModel
                {
                    IsSuccess = true,
                    Message = ex.Message
                });
            }
        }
        [HttpGet("GetUserByManagerId"), Authorize(Roles = "HR,Admin,Manager")]
        public async Task<IActionResult> GetUserByManagerOrHRIdAsync(string userId, int page, int pageSize = 10, string? searchValue = null)
        {
            try
            {
                var listUser = await _accountService.GetUserByManagerOrHRIdAsync(userId, page, pageSize, searchValue);
                if (listUser != null && listUser.userResponses.Any())
                {
                    return new OkObjectResult(new ResponseMessageViewModel
                    {
                        IsSuccess = true,
                        Data = listUser
                    });
                }
                else
                {
                    return new OkObjectResult(new ResponseMessageViewModel
                    {
                        IsSuccess = true,
                        Message = "User list not found"
                    });
                }
            }
            catch (Exception ex)
            {
                return new OkObjectResult(new ResponseMessageViewModel
                {
                    IsSuccess = true,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("GetUserByDepartmentIdAsync"), Authorize(Roles = "HR,Admin,Manager")]
        public async Task<IActionResult> GetUserByDepartmentIdAsync(int departmentId)
        {
            try
            {
                var listUser = await _accountService.GetUserByDepartmentIdAsync(departmentId);
                if (listUser != null && listUser.Any())
                {
                    return new OkObjectResult(new ResponseMessageViewModel
                    {
                        IsSuccess = true,
                        Data = listUser
                    });
                }
                else
                {
                    return new OkObjectResult(new ResponseMessageViewModel
                    {
                        IsSuccess = true,
                        Message = "User list not found"
                    });
                }
            }
            catch (Exception ex)
            {
                return new OkObjectResult(new ResponseMessageViewModel
                {
                    IsSuccess = true,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Check Manager Has Assigned User Or Not
        /// </summary>
        /// <param name="ManagerId"></param>
        /// <returns>If Manager has Assigned User then return True otherwise returns false</returns>

        [HttpGet("CheckAssignUsersByManager"), Authorize(Roles = "HR,Admin,Manager")]
        public async Task<IActionResult> CheckAssignUsersByManager(string ManagerId)
        {
            try
            {
                var users = await _accountService.CheckAssignUsersByManager(ManagerId);
                if (users)
                {
                    return new OkObjectResult(new ResponseMessageViewModel
                    {
                        IsSuccess = true,
                        Message = "Manager has Assigned Users"
                    });
                }
                else
                {
                    return new OkObjectResult(new ResponseMessageViewModel
                    {
                        IsSuccess = false,
                        Message = "Manager has not any assigned users"
                    });
                    // return new BadRequestObjectResult(new { Succeeded = false, Msg = "Manager list not found" });
                }

            }
            catch (Exception ex)
            {
                return new OkObjectResult(new ResponseMessageViewModel
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
        }
        /// <summary>
        /// Check Hr Has Assigned User Or Not
        /// </summary>
        /// <param name="ManagerId"></param>
        /// <returns>If Hr has Assigned User then return True otherwise returns false</returns>

        [HttpGet("CheckAssignUsersByHrId"), Authorize(Roles = "HR,Admin,Manager")]
        public async Task<IActionResult> CheckAssignUsersByHrId(string HrId)
        {
            try
            {
                var users = await _accountService.CheckAssignUsersByHrId(HrId);
                if (users)
                {
                    return new OkObjectResult(new ResponseMessageViewModel
                    {
                        IsSuccess = true,
                        Message = "Hr has Assigned Users"
                    });
                }
                else
                {
                    return new OkObjectResult(new ResponseMessageViewModel
                    {
                        IsSuccess = false,
                        Message = "Hr has not any assigned users"
                    });
                }

            }
            catch (Exception ex)
            {
                return new OkObjectResult(new ResponseMessageViewModel
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Update Manager if Manager IsActive status is false
        /// </summary>
        /// <param name="ManagerId"></param>
        /// <param name="NewManagerId"></param>
        /// <returns>If Manager is not active then Assigned Manager is updated by new Manager</returns>
        [HttpPost("UpdateManager"), Authorize(Roles = "HR,Admin")]
        public async Task<IActionResult> UpdateManager([FromBody] UpdateManagerViewModel updateManager)
        {
            try
            {
                var users = await _accountService.UpdateManager(updateManager);
                if (users.IsSuccess)
                {
                    return new OkObjectResult(new ResponseMessageViewModel
                    {
                        IsSuccess = true,
                        Data = null,
                        Message = "Manager Updated"
                    });
                }
                else
                {
                    return new OkObjectResult(new ResponseMessageViewModel
                    {
                        IsSuccess = false,
                        Message = "Faild to update manager"
                    });
                }

            }
            catch (Exception ex)
            {
                return new OkObjectResult(new ResponseMessageViewModel
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Get All Manager List
        /// </summary>
        /// <param name="Page"></param>
        /// <param name="PageSize"></param>
        /// <param name="SearchValue"></param>
        /// <returns></returns>
        [HttpGet("GetAllUserManager"), Authorize(Roles = "HR,Admin,Manager")]
        public async Task<IActionResult> GetAllUserManager([FromQuery] int Page, [FromQuery] int PageSize = 10, [FromQuery] string? SearchValue = null)
        {
            try
            {
                var result = await _accountService.GetAllUserManager(Page, PageSize, SearchValue);
                if (result != null)
                {
                    return new OkObjectResult(new ResponseMessageViewModel
                    {
                        IsSuccess = true,
                        Data = result
                    });
                }
                else
                {
                    return new OkObjectResult(new ResponseMessageViewModel
                    {
                        IsSuccess = false,
                        Message = "User list not found"
                    });
                }
            }
            catch (Exception ex)
            {
                return new OkObjectResult(new ResponseMessageViewModel
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
        }

    }
}
