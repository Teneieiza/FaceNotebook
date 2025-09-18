using Microsoft.AspNetCore.Mvc;
using FaceNoteBook.Services;
using FaceNoteBook.DTOs;
using FaceNoteBook.Utils;
using Microsoft.AspNetCore.Authorization;

namespace FaceNoteBook.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    // GET: api/users
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserResponseDto>>>> GetUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return ApiResponseHelper.Success(users);
    }

    // GET: api/users/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<UserResponseDto>>> GetUser(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {id} not found");

        return ApiResponseHelper.Success(user);
    }

    // GET: api/users/email/{email}
    [HttpGet("email/{email}")]
    public async Task<ActionResult<ApiResponse<UserResponseDto>>> GetUserByEmail(string email)
    {
        var user = await _userService.GetUserByEmailAsync(email);
        if (user == null)
            throw new KeyNotFoundException($"User with email {email} not found");

        return ApiResponseHelper.Success(user);
    }

    // POST: api/users
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<UserResponseDto>>> CreateUser(CreateUserDto createUserDto)
    {
        if (!ModelState.IsValid)
            throw new InvalidOperationException("Invalid user data");

        var user = await _userService.CreateUserAsync(createUserDto);
        return ApiResponseHelper.Success(user, 201, "User created successfully");
    }

    // PUT: api/users/profile/detail/{id}
    [HttpPut("profile/detail/{id}")]
    public async Task<ActionResult<ApiResponse<UserResponseDto>>> UpdateDetail(Guid id, [FromBody] UpdateDetailDto dto)
    {
        var user = await _userService.UpdateDetailAsync(id, dto);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {id} not found");
        
        return ApiResponseHelper.Success(user, 200, "Detail updated successfully");
    }

    // PUT: api/users/profile/email/{id}
    [HttpPut("profile/email/{id}")]
    public async Task<ActionResult<ApiResponse<UserResponseDto>>> UpdateEmail(Guid id, [FromBody] UpdateEmailDto dto)
    {
        var user = await _userService.UpdateEmailAsync(id, dto);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {id} not found");

        return ApiResponseHelper.Success(user, 200, "Email updated successfully");
    }

    // PUT: api/users/profile/password/{id}
    [HttpPut("profile/password/{id}")]
    public async Task<ActionResult<ApiResponse<UserResponseDto>>> UpdatePassword(Guid id, UpdatePasswordDto dto)
    {
            var user = await _userService.UpdatePasswordAsync(id, dto);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {id} not found");

            return ApiResponseHelper.Success(user, 200, "Password updated successfully");
    }

    // DELETE: api/users/5
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object?>>> DeleteUser(Guid id)
    {
        var deleted = await _userService.DeleteUserAsync(id);
        if (!deleted)
            throw new KeyNotFoundException($"User with ID {id} not found");

        return ApiResponseHelper.Success<object?>(null, 200, "User deleted successfully");
    }

    // GET: api/users/exists/5
    [HttpGet("exists/{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> UserExists(Guid id)
    {
        var exists = await _userService.UserExistsAsync(id);
        return ApiResponseHelper.Success(exists);
    }

    // POST: api/users/verify
    [HttpPost("verify")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<object>>> VerifyPassword([FromBody] UserLoginDto loginDto)
    {
        if (!ModelState.IsValid)
            throw new InvalidOperationException("Invalid login data");

        var isValid = await _userService.VerifyPasswordAsync(loginDto.Email, loginDto.Password);
        return ApiResponseHelper.Success<object>(new { IsValid = isValid });
    }
}
