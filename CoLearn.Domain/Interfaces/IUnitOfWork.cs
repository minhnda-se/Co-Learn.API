using CoLearn.Domain.Interfaces.Repositories;
using CoLearn.Infrastructure.Models;
using System;
using System.Threading.Tasks;

namespace CoLearn.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        //  Repositories riêng
        IUserProfileRepository UserProfileCustom { get; }

        // Commit tất cả thay đổi
        Task<int> CommitAsync();
    }
}
