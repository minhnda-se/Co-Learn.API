using CoLearn.Domain.Interfaces;
using CoLearn.Domain.Interfaces.Repositories;
using CoLearn.Infrastructure.Context;
using CoLearn.Infrastructure.Models;
using CoLearn.Infrastructure.Repositories;
using System.Threading.Tasks;

namespace CoLearn.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        // backing fields cho lazy init
        private IUserProfileRepository _userProfileRepository;
        private IGenericRepository<UserProfile> _userProfileGenericRepository;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        // Custom repository
        public IUserProfileRepository UserProfileRepository
            => _userProfileRepository ??= new UserProfileRepository(_context);

        // Generic repository
        public IGenericRepository<UserProfile> UserProfileGenericRepository
            => _userProfileGenericRepository ??= new GenericRepository<UserProfile>(_context);

        // Commit tất cả thay đổi
        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // Dispose DbContext
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
