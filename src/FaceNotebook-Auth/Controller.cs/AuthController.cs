using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FaceNoteBook.Data;
using FaceNoteBook.DTOs;
using FaceNoteBook.Services;
using FaceNoteBook.Utils;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FaceNoteBook.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly UserDataContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthController(IUserService userService, UserDataContext context, IPasswordHasher passwordHasher, ITokenService tokenService)
    {
        _userService = userService;
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

// POST: api/auth/login
[HttpPost("login")]
[AllowAnonymous]
public async Task<ActionResult<ApiResponse<object>>> Login([FromBody] UserLoginDto dto)
  {
    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
    if (user == null || !await _passwordHasher.VerifyHashedPasswordAsync(dto.Password, user.Password))
    {
        return ApiResponseHelper.Fail("Invalid email or password", 401);
    }

    var accessToken = _tokenService.GenerateAccessToken(user);
    var refreshToken = _tokenService.GenerateRefreshToken();

    user.RefreshToken = refreshToken;
    user.RefreshTokenExpiryTime = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(7), DateTimeKind.Unspecified);

    _context.Entry(user).Property(u => u.RefreshToken).IsModified = true;
    _context.Entry(user).Property(u => u.RefreshTokenExpiryTime).IsModified = true;

    await _context.SaveChangesAsync();

    return ApiResponseHelper.Success<object>(new { accessToken, refreshToken });
  }

// POST: api/auth/refresh
[HttpPost("refresh")]
[Authorize]
public async Task<ActionResult<ApiResponse<object>>> Refresh([FromBody] string refreshToken)
  {
    var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
    if (user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
    {
        return ApiResponseHelper.Fail("Invalid or expired refresh token", 401);
    }

    var newAccessToken = _tokenService.GenerateAccessToken(user);
    var newRefreshToken = _tokenService.GenerateRefreshToken();

    user.RefreshToken = newRefreshToken;
    user.RefreshTokenExpiryTime = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(7), DateTimeKind.Unspecified);

    _context.Entry(user).Property(u => u.RefreshToken).IsModified = true;
    _context.Entry(user).Property(u => u.RefreshTokenExpiryTime).IsModified = true;

    await _context.SaveChangesAsync();

    return ApiResponseHelper.Success<object>(new { accessToken = newAccessToken, refreshToken = newRefreshToken });
  }

// POST: api/auth/logout
[HttpPost("logout")]
[Authorize]
public async Task<ActionResult<ApiResponse<object>>> Logout([FromBody] string refreshToken)
  {
      var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

      if (string.IsNullOrEmpty(userIdClaim))
          return ApiResponseHelper.Fail("Invalid token", 401);

      var userId = Guid.Parse(userIdClaim);

      var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && u.RefreshToken == refreshToken);
      if (user == null)
          return ApiResponseHelper.Fail("Invalid or expired refresh token", 401);

      user.RefreshToken = null;
      user.RefreshTokenExpiryTime = null;
      await _context.SaveChangesAsync();

      return ApiResponseHelper.Success<object>(null, 200, "Logout successful");
  }
}
