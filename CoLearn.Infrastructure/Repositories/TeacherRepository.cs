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
    public class TeacherRepository : GenericRepository<Teacher>, ITeacherRepository
    {
        public TeacherRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<List<Teacher>> GetAllTeachersAsync()
        {
            return await _context.Teachers
                .Where(t => !t.IsDeleted)
                .Include(t => t.User).ThenInclude(t => t.UserProfile)
                .ToListAsync();
        }

        public async Task<Teacher> GetByUserIdAsync(int userId)
        {
            return await _context.Teachers
               .Include(t => t.User).ThenInclude(t => t.UserProfile)
               .FirstOrDefaultAsync(t => t.UserId == userId);
        }

        public async Task<Teacher> GetTeacherByIdAsync(int teacherId)
        {
            return await _context.Teachers
                .Include(t => t.User).ThenInclude(t => t.UserProfile)
                .FirstOrDefaultAsync(t => t.TeacherId == teacherId);
        }
    }
}


