using FaceNoteBook.Utils;
using FaceNoteBook.Services;
// using TodoList.Services;

namespace FaceNoteBook.Extensions
{
    public static class ScopedServiceExtensions
    {
        public static IServiceCollection AddScopedServices(this IServiceCollection services)
        {
            // Services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            // services.AddScoped<ITodoService, TodoService>();

            // Utils
            services.AddScoped<IPasswordHasher, PasswordHasher>();

            return services;
        }
    }
}
