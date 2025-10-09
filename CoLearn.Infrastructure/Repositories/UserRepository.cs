using CoLearn.Domain.Common;
using CoLearn.Domain.Interfaces.Repositories;
using CoLearn.Domain.Models;
using CoLearn.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<PagedResult<User>> GetAllAsync(int pageIndex, int pageSize)
        {
            var query = _context.Users.AsQueryable();
            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<User>(items, pageIndex, pageSize, totalCount);
        }

        public async Task<User> CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> UpdateAsync(User user)
        {
            var existingUser = await _context.Users.FindAsync(user.UserId);
            if (existingUser == null)
                return null;

            _context.Entry(existingUser).CurrentValues.SetValues(user);
            await _context.SaveChangesAsync();

            return existingUser;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;
            user.Email = $"deleted_{user.UserId}_{Guid.NewGuid()}@example.com";
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.PrimaryRole)
                .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
        }

        public async Task<User?> GetByVerificationTokenAsync(string token)
        => await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == token && !u.IsDeleted);
    }
}
