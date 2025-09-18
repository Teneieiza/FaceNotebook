using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;

namespace FaceNoteBook.Extensions
{
    public static class JwtAuthenticationExtensions
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var key = configuration["Jwt:Key"] 
                      ?? throw new ArgumentNullException("Jwt:Key not found");
            var issuer = configuration["Jwt:Issuer"] ?? "facenotebook_app";
            var audience = configuration["Jwt:Audience"] ?? "earth_ten";

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
              options.RequireHttpsMetadata = false;
              options.SaveToken = true;
              options.TokenValidationParameters = new TokenValidationParameters
              {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
              };
              options.Events = new JwtBearerEvents
              {
                  OnChallenge = context =>
                  {
                      context.HandleResponse();
                      context.Response.StatusCode = 401;
                      context.Response.ContentType = "application/json";

                      var result = JsonSerializer.Serialize(new
                      {
                          status = 401,
                          message = "Invalid or expired token"
                      });

                      return context.Response.WriteAsync(result);
                  }
              };
            });

            services.AddAuthorization();

            return services;
        }
    }
}
