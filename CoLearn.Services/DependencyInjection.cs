using Microsoft.Extensions.DependencyInjection;

namespace CoLearn.Services
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            // Đăng ký Application Services ở đây
            // services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}
