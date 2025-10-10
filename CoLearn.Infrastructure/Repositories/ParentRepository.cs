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
    public class ParentRepository : GenericRepository<Parent>, IParentRepository
    {
        public ParentRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<List<Parent>> GetAllParentsAsync()
        {
            return await _context.Parents
                .Include(p => p.User)
                    .ThenInclude(u => u.UserProfile)
                .Include(p => p.Students) 
                    .ThenInclude(s => s.User) 
                        .ThenInclude(u => u.UserProfile)
                .ToListAsync();
        }

        public async Task<Parent> GetByUserIdAsync(int? userId)
        {
            return await _context.Parents
               .Include(p => p.User)
                   .ThenInclude(u => u.UserProfile)
               .Include(p => p.Students)
                   .ThenInclude(s => s.User)
                       .ThenInclude(u => u.UserProfile)
               .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<Parent> GetParentByIdAsync(int? parentId)
        {
            return await _context.Parents
                .Include(p => p.User)
                    .ThenInclude(u => u.UserProfile)
                .Include(p => p.Students)
                    .ThenInclude(s => s.User)
                        .ThenInclude(u => u.UserProfile)
                .FirstOrDefaultAsync(p => p.ParentId == parentId);
        }
    }
}
