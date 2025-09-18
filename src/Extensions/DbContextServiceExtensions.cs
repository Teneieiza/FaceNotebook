using FaceNoteBook.Data;
using Microsoft.EntityFrameworkCore;

namespace FaceNoteBook.Extensions
{
    public static class DbContextServiceExtensions
    {
        public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<UserDataContext>(options =>
                options.UseNpgsql(Environment.GetEnvironmentVariable("DB_CONNECTION"))
            );

            return services;
        }
    }
}
