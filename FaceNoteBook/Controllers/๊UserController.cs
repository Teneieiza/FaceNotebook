using Microsoft.AspNetCore.Mvc;
using FaceNoteBook.Services;
using FaceNoteBook.DTOs;

namespace FaceNoteBook.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    // GET: api/users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsers()
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting users");
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    // GET: api/users/5
    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponseDto>> GetUser(Guid id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found");
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting user {UserId}", id);
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    // GET: api/users/email/{email}
    [HttpGet("email/{email}")]
    public async Task<ActionResult<UserResponseDto>> GetUserByEmail(string email)
    {
        try
        {
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"User with email {email} not found");
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting user by email {Email}", email);
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    // POST: api/users
    [HttpPost]
    public async Task<ActionResult<UserResponseDto>> CreateUser(CreateUserDto createUserDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.CreateUserAsync(createUserDto);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating user");
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    // PUT: api/users/5
    [HttpPut("{id}")]
    public async Task<ActionResult<UserResponseDto>> UpdateUser(Guid id, UpdateUserDto updateUserDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.UpdateUserAsync(id, updateUserDto);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found");
            }

            return Ok(user);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating user {UserId}", id);
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    // DELETE: api/users/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        try
        {
            var deleted = await _userService.DeleteUserAsync(id);
            if (!deleted)
            {
                return NotFound($"User with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting user {UserId}", id);
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    // GET: api/users/exists/5
    [HttpGet("exists/{id}")]
    public async Task<ActionResult<bool>> UserExists(Guid id)
    {
        try
        {
            var exists = await _userService.UserExistsAsync(id);
            return Ok(exists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while checking user existence {UserId}", id);
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    // POST: api/users/verify
    [HttpPost("verify")]
    public async Task<ActionResult<bool>> VerifyPassword([FromBody] UserLoginDto loginDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var isValid = await _userService.VerifyPasswordAsync(loginDto.Email, loginDto.Password);
            return Ok(new { IsValid = isValid });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while verifying password for {Email}", loginDto.Email);
            return StatusCode(500, "An error occurred while processing your request");
        }
    }
}