using FaceNoteBook.Data;
using FaceNoteBook.Services;
using Microsoft.EntityFrameworkCore;

namespace FaceNoteBook.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            var connStr = config["DB_CONNECTION"];

            if (string.IsNullOrEmpty(connStr))
            {
                connStr = Environment.GetEnvironmentVariable("DB_CONNECTION");
            }

            Console.WriteLine($"DB_CONNECTION = {connStr}");

            services.AddDbContext<UserDataContext>(options =>
                options.UseNpgsql(connStr)
            );

            // Register services
            services.AddScoped<IUserService, UserService>();

            return services;
        }

    }
}
