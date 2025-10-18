using CoLearn.Domain.Interfaces.Services;
using CoLearn.Services.Handler;
using CoLearn.Services.Implementations;
using CoLearn.Services.Mappings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CoLearn.Services
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Đăng ký Application Services ở đây

            // AutoMapper
            services.AddAutoMapper(typeof(UserMapping).Assembly);
            // Đăng ký Service
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserProfileService, UserProfileService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<ITeacherService, TeacherService>();
            services.AddScoped<IParentService, ParentService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<ILessonService, LessonService>();
            services.AddScoped<IAssignmentService, AssignmentService>();
            services.AddScoped<ICourseMaterialService, CourseMaterialService>();
            services.AddScoped<IEnrollmentService, EnrollmentService>();
            services.AddScoped<ISubmissionService, SubmissionService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<BookingJobHandler>();
            services.AddScoped<IPaymentService, PaymentService>();
            // JWT Auth
            var jwtSettings = configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            return services;
        }
    }
}
