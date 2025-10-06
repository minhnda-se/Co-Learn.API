using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CoLearn.Domain.Interfaces
{
    public interface IBackgroundJobService
    {
        string Enqueue<T>(Expression<Func<T, Task>> methodCall);
        string Schedule<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay);
        bool Delete(string jobId);
    }
}
