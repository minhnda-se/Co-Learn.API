using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CoLearn.Infrastructure.Context;
using Microsoft.Extensions.Options;
using CoLearn.Domain.Interfaces;
using CoLearn.Infrastructure.Repositories;
using CoLearn.Infrastructure.Services;
using Amazon.S3;
using CoLearn.Domain.Interfaces.Services;
using CoLearn.Infrastructure.Notifications;
using Hangfire;
using Hangfire.SqlServer;
using CoLearn.Infrastructure.Services.BackgroundJobs;


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
            // AWS S3
            services.AddScoped<IS3StorageService, S3StorageService>();
            // Stmp Email
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.AddScoped<INotificationService, EmailNotificationService>();
            // Background Job with Hangfire
            services.AddScoped<IBackgroundJobService, HangfireBackgroundJobService>();
            services.AddHangfire(config => config
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
                    {
                        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                        QueuePollInterval = TimeSpan.Zero,
                        UseRecommendedIsolationLevel = true,
                        DisableGlobalLocks = true
                    }));

            // Thêm server xử lý background jobs
            services.AddHangfireServer();

            return services;
        }
    }
}
