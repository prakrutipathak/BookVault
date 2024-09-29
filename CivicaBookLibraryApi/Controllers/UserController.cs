using CivicaBookLibraryApi.Dtos;
using CivicaBookLibraryApi.Services.Contract;
using CivicaBookLibraryApi.Services.Implementation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CivicaBookLibraryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }


        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(LoginDto login)
        {
            var response = _userService.LoginUserService(login);
            return !response.Success ? BadRequest(response) : Ok(response);
        }


        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult RegisterUser(RegisterDto registerDto)
        {

            var result = _userService.RegisterUserService(registerDto);

            return !result.Success ? BadRequest(result) : Ok(result);
        }

        [AllowAnonymous]
        [HttpPut("ResetPassword")]
        public IActionResult ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var response = _userService.ResetPassword(resetPasswordDto);
            return !response.Success ? BadRequest(response) : Ok(response);
        }


        [Authorize]
        [HttpGet("GetUserByLoginId/{id}")]
        public IActionResult GetUserById(int id)
        {
            var response = _userService.GetUserById(id);
            if (!response.Success)
            {
                return NotFound(response);
            }
            else
            {
                return Ok(response);
            }

        }


        [Authorize]
        [HttpPut("ChangePassword")]
        public IActionResult ChangePassword(ChangePasswordDto changePasswordDto)
        {
            var response = _userService.ChangePassword(changePasswordDto);
            return !response.Success ? BadRequest(response) : Ok(response);
        }


        [Authorize]
        [HttpGet("GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            var response = _userService.GetAllUsers();
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }


        [Authorize]
        [HttpGet("GetAllUsersByPagination")]
        public IActionResult GetPaginatedUSer(string? search, int page = 1, int pageSize = 4, string sortOrder = "asc")
        {
            var response = new ServiceResponse<IEnumerable<UserDto>>();
            response = _userService.GetPaginatedUsers(page, pageSize, search, sortOrder);
            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpGet("GetUsersCount")]
        public IActionResult GetTotalCountOfUsers(string? search)
        {
            var response = _userService.TotalUsers(search);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [Authorize]
        [HttpDelete("DeleteUser/{id}")]
        public IActionResult DeleteUser(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Please enter valid data.");
            }
            else
            {
                var response = _userService.RemoveUser(id);
                return response.Success ? Ok(response) : BadRequest(response);
            }

        }
    }
}
