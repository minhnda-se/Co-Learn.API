using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CoLearn.Infrastructure.Context;
using CoLearn.Domain.Interfaces;
using CoLearn.Infrastructure.Repositories;
using CoLearn.Infrastructure.Services;
using Amazon.S3;
using CoLearn.Domain.Interfaces.Services;
using CoLearn.Infrastructure.Notifications;
using Hangfire;
using Hangfire.SqlServer;
using CoLearn.Infrastructure.Services.BackgroundJobs;
using CoLearn.Infrastructure.Services.Payments;

namespace CoLearn.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // ✅ Cho API (Scoped)
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            // ✅ Cho background jobs (factory an toàn)
            services.AddDbContextFactory<AppDbContext>(options =>
                options.UseSqlServer(connectionString),
                lifetime: ServiceLifetime.Scoped // 👈 KHÁC BIỆT QUAN TRỌNG
            );

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IS3StorageService, S3StorageService>();
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.AddScoped<INotificationService, EmailNotificationService>();

            // ✅ Hangfire config
            services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));

            services.AddHangfireServer();

            // ✅ Background job service
            services.AddScoped<IBackgroundJobService, HangfireBackgroundJobService>();

            // ✅ Payment services
            services.AddScoped<IPayOSService, PayOSService>();

            return services;
        }
    }
}
