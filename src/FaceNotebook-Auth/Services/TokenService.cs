using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FaceNoteBook.Models;
using Microsoft.IdentityModel.Tokens;

namespace FaceNoteBook.Services;

public class TokenService : ITokenService
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _accessTokenExpirationMinutes;
    private readonly int _refreshTokenExpirationDays;

    public TokenService(IConfiguration configuration)
    {
        _secretKey = configuration["Jwt:Key"] 
            ?? throw new ArgumentNullException("Jwt:Key not found");

        _issuer = configuration["Jwt:Issuer"] ?? "facenotebook_app";
        _audience = configuration["Jwt:Audience"] ?? "earth_ten";
        _accessTokenExpirationMinutes = int.TryParse(configuration["Jwt:AccessTokenMinutes"], out var at)
            ? at : 15;
        _refreshTokenExpirationDays = int.TryParse(configuration["Jwt:RefreshTokenDays"], out var rt)
            ? rt : 7;
    }

    public string GenerateAccessToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("name", user.Name ?? "")
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_accessTokenExpirationMinutes),
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature),
            Issuer = _issuer,
            Audience = _audience
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    //--------------------- TokenExpiry -------------------
    public ClaimsPrincipal ExtractUserFromExpiredToken(string expiredToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_secretKey);

        try
        {
            var extract = tokenHandler.ValidateToken(expiredToken, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            }, out _);

            return extract;
        }
        catch
        {
            throw new SecurityTokenException("Invalid token format");
        }
    }

    public DateTime GetAccessTokenExpiry()
    {
        var utcTime = DateTime.UtcNow.AddMinutes(_accessTokenExpirationMinutes);
        return DateTime.SpecifyKind(utcTime, DateTimeKind.Unspecified);
    }

    public DateTime GetRefreshTokenExpiry()
    {
        var utcTime = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays);
        return DateTime.SpecifyKind(utcTime, DateTimeKind.Unspecified);
    }
}
