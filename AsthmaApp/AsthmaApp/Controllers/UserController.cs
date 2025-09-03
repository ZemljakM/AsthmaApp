using AsthmaApp.Common;
using AsthmaApp.Service.Common;
using AsthmaApp.WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Security.Claims;
using System.Text.RegularExpressions;
using static BCrypt.Net.BCrypt;

namespace AsthmaApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        #region Fields

        private IAuthService _authService;
        private IUserService _userService;
        private IRoleService _roleService;

        #endregion Fields

        #region Constructors

        public UserController(IAuthService authService, IUserService userService, IRoleService roleService)
        {
            _authService = authService;
            _userService = userService;
            _roleService = roleService;
        }

        #endregion Constructors

        #region Methods

        [HttpPut]
        [Authorize(Roles = "Admin")]
        [Route("{id}")]
        public async Task<IActionResult> DeactivateUserAsync(Guid id)
        {
            var result = await _userService.DeactivateUserAsync(id);

            return result ? Ok() : BadRequest();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsersAsync(string searchQuery = "", bool? isApproved = null, int pageSize = 6, 
            int pageNumber = 1, string orderBy = "LastName", string orderDirection = "ASC")
        {
            Filter filter = new Filter
            {
                SearchQuery = searchQuery,
                IsApproved = isApproved
            };

            Paging paging = new Paging
            {
                PageSize = pageSize,
                PageNumber = pageNumber
            };

            Sorting sorting = new Sorting
            {
                OrderBy = orderBy,
                OrderDirection = orderDirection
            };

            var result = await _userService.GetAllUsersAsync(filter, paging, sorting);

            return Ok(result);
        }


        [HttpGet]
        [Authorize(Roles = "Admin, Doctor")]
        [Route("count-users")]
        public async Task<IActionResult> CountUsersAsync(string searchQuery = "", bool? isApproved = null, bool? isActive = null, Guid? userId = null, Guid? doctorId = null)
        {
            Filter filter = new Filter
            {
                SearchQuery = searchQuery,
                IsApproved = isApproved,
                IsActive = isActive,
                UserId = userId,
                DoctorId = doctorId
            };

            var result = await _userService.CountUsersAsync(filter);

            return Ok(result);
        }

        [HttpGet]
        [Route("patients/{doctorId}")]
        [Authorize(Roles = "Admin, Doctor")]
        public async Task<IActionResult> GetPatientsForDoctorAsync(Guid doctorId, int pageSize = 3,
            int pageNumber = 1, string orderBy = "LastName", string orderDirection = "ASC")
        {
            Paging paging = new Paging
            {
                PageSize = pageSize,
                PageNumber = pageNumber
            };

            Sorting sorting = new Sorting
            {
                OrderBy = orderBy,
                OrderDirection = orderDirection
            };

            var result = await _userService.GetPatientsForDoctorAsync(doctorId, paging, sorting);

            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize(Roles = "Admin, Doctor, Patient")]
        public async Task<IActionResult> GetUserByIdAsync(Guid id)
        {
            var currentUserId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var currentUserRole = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            var user = await _userService.GetUserByIdAsync(id);

            if (user is null)
                return NotFound();

            if (currentUserRole == "Admin")
            {
                return Ok(user);
            }
            else if (currentUserRole == "Patient")
            {
                if (currentUserId == id)
                {
                    return Ok(user); 
                }
                else
                {
                    return Forbid();
                }
            }
            else if (currentUserRole == "Doctor")
            {
                if (currentUserId == id || user.DoctorId == currentUserId)
                {
                    return Ok(user);
                }
                else
                {
                    return Forbid();
                }
            }

            return Forbid();
        }

        [HttpGet]
        [Route("patient/{oib}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetUserByOIBAsync(string oib)
        {
            var user = await _userService.GetUserByOIBAsync(oib);

            return user is not null ? Ok(user) : NotFound();
        }

        [HttpGet]
        [Route("check-profile")]
        [Authorize(Roles = "Admin, Doctor, Patient")]
        public async Task<IActionResult> IsProfileCompletedAsync()
        {
            var userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var result = await _userService.IsProfileCompletedAsync(userId);

            return result ? Ok() : BadRequest("Profile not completed.");
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterAsync(RegistrationModel registrationModel)
        {
            var existingUser = await _userService.GetUserByEmailAsync(registrationModel.Email);

            if (existingUser is not null) 
            {
                return Conflict();
            }

            if (!IsEmailAddressValid(registrationModel.Email) || !Regex.IsMatch(registrationModel.OIB, @"^\d{11}$"))
            {
                return BadRequest();
            }

            var user = await _userService.CreateUserAsync();
            user.FirstName = registrationModel.FirstName;
            user.LastName = registrationModel.LastName;
            user.OIB = registrationModel.OIB;
            user.Email = registrationModel.Email;
            user.Password = HashPassword(registrationModel.Password);
            user.Role = await _roleService.GetRoleByNameAsync(registrationModel.RoleName);
            user.IsApproved = String.Equals(registrationModel.RoleName, "Patient") ? true : false;


            var result = await _userService.InsertUserAsync(user);
            if (result is null || result == Guid.Empty)
            {
                return BadRequest();
            }

            if (registrationModel.RoleName == "Doctor")
            {
                return Ok(); 
            }

            var token = await _authService.GenerateTokenAsync(user);

            return Ok(new { Token = token });
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginAsync(LoginModel loginModel)
        {
            var user = await _userService.AuthenticateAsync(loginModel.Email, loginModel.Password);

            if (user == null)
            {
                return NotFound();
            }

            var token = await _authService.GenerateTokenAsync(user);
            return Ok(new { Token = token });
        }

        [HttpPut]
        [Authorize(Roles = "Admin, Doctor, Patient")]
        public async Task<IActionResult> UpdateUserAsync(UserModel userModel)
        {
            var loggedInUserRole = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            if (userModel is null)
            {
                return NotFound();
            }

            var user = await _userService.GetUserByIdAsync(userModel.Id);
            if (userModel.FirstName != null)
            {
                user.FirstName = userModel.FirstName;
            }
            if (userModel.LastName != null)
            {
                user.LastName = userModel.LastName;
            }
            if (userModel.DateOfBirth.HasValue)
            {
                user.DateOfBirth = userModel.DateOfBirth.Value;
            }

            if (!string.IsNullOrEmpty(userModel.Gender))
            {
                user.Gender = userModel.Gender;
            }

            if (userModel.EthnicityId.HasValue && userModel.EthnicityId != Guid.Empty)
            {
                user.EthnicityId = userModel.EthnicityId;
            }

            if (userModel.EducationLevelId.HasValue && userModel.EducationLevelId != Guid.Empty)
            {
                user.EducationLevelId = userModel.EducationLevelId;
            }

            if ((userModel.DoctorId.HasValue && userModel.DoctorId != Guid.Empty) || userModel.IsDoctorEdited.GetValueOrDefault())
            {
                user.DoctorId = userModel.DoctorId;
            }
            if (userModel.IsActive != null)
            {
                user.IsActive = (bool)userModel.IsActive;
            }
            if (userModel.IsApproved != null && loggedInUserRole == "Admin")
            {
                user.IsApproved = (bool)userModel.IsApproved;
            }

            var result = await _userService.UpdateUserAsync(user);

            return result ? Ok() : BadRequest();
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        [Route("{id}")]
        public async Task<IActionResult> DeleteUserAsync(Guid id)
        {
            var result = await _userService.DeleteUserAsync(id);


            return result ? Ok() : BadRequest();
        }

        private bool IsEmailAddressValid(string email)
        {
            try
            {
                var address = new MailAddress(email);
                if (address.Address != email)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion Methods
    }
}
