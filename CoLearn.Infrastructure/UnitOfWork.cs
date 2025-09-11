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

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            //Khởi tạo các genericrepository
            UserProfileGenericRepository = new GenericRepository<UserProfile>(_context);

            // Khởi tạo repository
            UserProfileRepository = new UserProfileRepository(_context);
        }
        public IUserProfileRepository UserProfileRepository { get; private set; }
        public IGenericRepository<UserProfile> UserProfileGenericRepository { get; private set; }


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
