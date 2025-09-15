using TodoList.Services;

namespace TodoList.Extensions;

public static class ServicesExtension
{
  public static IServiceCollection AddApplicationServices(this IServiceCollection services)
  {
    services.AddScoped<ITodoService, TodoService>();
    return services;
  }
}