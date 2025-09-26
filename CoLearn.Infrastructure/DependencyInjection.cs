using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CoLearn.Infrastructure.Context;
using CoLearn.Domain.Interfaces;
using CoLearn.Infrastructure.Repositories;
using CoLearn.Infrastructure.Services;
using Amazon.S3;

namespace CoLearn.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Đăng ký Repository + UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IS3StorageService, S3StorageService>();
            // AWS S3
            services.AddScoped<IS3StorageService, S3StorageService>();


            return services;
        }
    }
}
