using CoLearn.Domain.Interfaces.Services;
using CoLearn.Services.Implementations;
using CoLearn.Services.Mappings;
using Microsoft.Extensions.DependencyInjection;

namespace CoLearn.Services
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            // Đăng ký Application Services ở đây
            // AutoMapper
            services.AddAutoMapper(typeof(UserMapping).Assembly);
            // Đăng ký Service
            //services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserProfileService, UserProfileService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<ITeacherService, TeacherService>();
            services.AddScoped<IParentService, ParentService>();

            return services;
        }
    }
}
