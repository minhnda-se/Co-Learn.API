using Hangfire;
using CoLearn.Domain.Enums;
using CoLearn.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using CoLearn.Infrastructure.Context;
using CoLearn.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CoLearn.Infrastructure.Services.BackgroundJobs
{
    public class HangfireBackgroundJobService : IBackgroundJobService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public HangfireBackgroundJobService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        // 🔹 Enqueue job chạy ngay (không có DI)
        public string Enqueue(Func<Task> methodCall, JobType jobType, string targetType, int? targetId = null)
        {
            var jobId = BackgroundJob.Enqueue(() => methodCall());
            _ = SaveJobAsync(jobId, jobType, targetType, targetId, TimeSpan.Zero);
            return jobId;
        }

        // 🔹 Schedule job chạy sau delay (không có DI)
        public string Schedule(Func<Task> methodCall, TimeSpan delay, JobType jobType, string targetType, int? targetId = null)
        {
            var jobId = BackgroundJob.Schedule(() => methodCall(), delay);
            _ = SaveJobAsync(jobId, jobType, targetType, targetId, delay);
            return jobId;
        }

        // 🔹 Enqueue job có DI (VD: BookingJobHandler)
        public string Enqueue<T>(Expression<Func<T, Task>> methodCall, JobType jobType, string targetType, int? targetId = null)
        {
            var jobId = BackgroundJob.Enqueue(methodCall);
            _ = SaveJobAsync(jobId, jobType, targetType, targetId, TimeSpan.Zero);
            return jobId;
        }

        // 🔹 Schedule job có DI (VD: BookingJobHandler)
        public string Schedule<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay, JobType jobType, string targetType, int? targetId = null)
        {
            var jobId = BackgroundJob.Schedule(methodCall, delay);
            _ = SaveJobAsync(jobId, jobType, targetType, targetId, delay);
            return jobId;
        }

        // 🔹 Xóa job theo Hangfire JobID
        public bool Delete(string jobId)
        {
            if (string.IsNullOrEmpty(jobId))
                return false;

            var result = BackgroundJob.Delete(jobId);
            if (!result) return false;

            using var context = _contextFactory.CreateDbContext();

            var job = context.Jobs.FirstOrDefault(j => j.HangfireJobId == jobId);
            if (job != null)
            {
                job.Status = "Deleted";
                job.UpdatedAt = DateTime.UtcNow;
                context.SaveChanges();
            }

            return true;
        }

        // 🔹 Xóa toàn bộ job liên quan đến 1 đối tượng (VD: BookingID = 12)
        public async Task<int> DeleteByTargetAsync(string targetType, int targetId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            var targetJobs = await context.JobTargets
                .Include(t => t.Job)
                .Where(t => t.TargetType == targetType && t.TargetId == targetId)
                .ToListAsync();

            int deletedCount = 0;

            foreach (var jobTarget in targetJobs)
            {
                var hangfireJobId = jobTarget.Job?.HangfireJobId;
                if (!string.IsNullOrEmpty(hangfireJobId))
                {
                    BackgroundJob.Delete(hangfireJobId);
                    deletedCount++;
                }

                if (jobTarget.Job != null)
                {
                    jobTarget.Job.Status = "Deleted";
                    jobTarget.Job.UpdatedAt = DateTime.UtcNow;
                }
            }

            context.JobTargets.RemoveRange(targetJobs);
            await context.SaveChangesAsync();

            return deletedCount;
        }

        // 🔹 Lưu job và mapping target
        private async Task SaveJobAsync(string hangfireJobId, JobType jobType, string targetType, int? targetId, TimeSpan delay)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();

                var job = new Job
                {
                    JobType = jobType.ToString(),
                    HangfireJobId = hangfireJobId,
                    Status = "Pending",
                    RunAt = DateTime.UtcNow.Add(delay),
                    CreatedAt = DateTime.UtcNow,
                    JobTargets = new List<JobTarget>()
                };

                if (targetId.HasValue && !string.IsNullOrEmpty(targetType))
                {
                    job.JobTargets.Add(new JobTarget
                    {
                        TargetType = targetType,
                        TargetId = targetId.Value,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                context.Jobs.Add(job);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HangfireBackgroundJobService] Error saving job log: {ex.Message}");
            }
        }

        // Job xóa user chưa xác thực sau 5p
        
    }
}
