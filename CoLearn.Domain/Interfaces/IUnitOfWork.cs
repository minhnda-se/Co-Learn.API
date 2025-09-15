using CoLearn.Domain.Interfaces.Repositories;
using CoLearn.Domain.Models;
using System;
using System.Threading.Tasks;

namespace CoLearn.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Generic Repositories
        IGenericRepository<UserProfile> UserProfileGenericRepository { get; }
        IGenericRepository<User> UserGenericRepository { get; }

        //  Repositories 
        IUserProfileRepository UserProfileRepository { get; }
        IUserRepository UserRepository { get; }

        // Commit tất cả thay đổi
        Task<int> CommitAsync();
    }
}
