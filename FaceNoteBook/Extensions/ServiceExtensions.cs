using FaceNoteBook.Data;
using FaceNoteBook.Services;
using Microsoft.EntityFrameworkCore;

namespace FaceNoteBook.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<UserDataContext>(options =>
                options.UseNpgsql(Environment.GetEnvironmentVariable("DB_CONNECTION"))
            );

            // เพิ่ม service อื่นๆตรงนี้
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}
