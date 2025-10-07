using CoLearn.Domain.Enums;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CoLearn.Domain.Interfaces
{
    public interface IBackgroundJobService
    {
        // 🔹 Job trực tiếp (Func<Task>)
        string Enqueue(Func<Task> methodCall, JobType jobType, string targetType, int? targetId = null);
        string Schedule(Func<Task> methodCall, TimeSpan delay, JobType jobType, string targetType, int? targetId = null);

        // 🔹 Job sử dụng DI (VD: BookingJobHandler)
        string Enqueue<T>(Expression<Func<T, Task>> methodCall, JobType jobType, string targetType, int? targetId = null);
        string Schedule<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay, JobType jobType, string targetType, int? targetId = null);

        // 🔹 Xóa job theo Hangfire JobID
        bool Delete(string jobId);

        // 🔹 Xóa toàn bộ job liên quan đến 1 entity cụ thể (VD: Booking)
        Task<int> DeleteByTargetAsync(string targetType, int targetId);
    }
}
