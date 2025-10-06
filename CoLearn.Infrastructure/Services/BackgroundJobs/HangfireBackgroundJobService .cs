using CoLearn.Domain.Interfaces;
using Hangfire;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CoLearn.Infrastructure.Services.BackgroundJobs
{
    public class HangfireBackgroundJobService : IBackgroundJobService
    {
        public string Enqueue<T>(Expression<Func<T, Task>> methodCall)
        {
            return BackgroundJob.Enqueue(methodCall);
        }

        public string Schedule<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay)
        {
            return BackgroundJob.Schedule(methodCall, delay);
        }

        public bool Delete(string jobId)
        {
            return BackgroundJob.Delete(jobId);
        }
    }
}
