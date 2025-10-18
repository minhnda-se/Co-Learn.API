using CoLearn.Domain.Interfaces.Repositories;
using CoLearn.Domain.Models;
using CoLearn.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoLearn.Infrastructure.Repositories
{
    public class StudentRepository : GenericRepository<Student>, IStudentRepository
    {
        public StudentRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<Student>> GetAllStudentsAsync()
        {
            return await _context.Students
                .Where(s => !s.IsDeleted && s.User != null && !s.User.IsDeleted)
                .Include(s => s.User)
                    .ThenInclude(u => u.UserProfile)
                .Include(s => s.Parent)
                    .ThenInclude(p => p.User)
                        .ThenInclude(u => u.UserProfile)
                .ToListAsync();
        }

        public async Task<List<Student>> GetByParentIdAsync(int parentId)
        {
            return await _context.Students
                .Where(s => !s.IsDeleted
                            && s.ParentId == parentId
                            && s.User != null && !s.User.IsDeleted
                            && s.Parent != null && !s.Parent.IsDeleted
                            && s.Parent.User != null && !s.Parent.User.IsDeleted)
                .Include(s => s.User)
                    .ThenInclude(u => u.UserProfile)
                .Include(s => s.Parent)
                    .ThenInclude(p => p.User)
                        .ThenInclude(u => u.UserProfile)
                .ToListAsync();
        }

        public async Task<Student> GetByUserIdAsync(int userId)
        {
            return await _context.Students
                .Where(s => !s.IsDeleted && s.UserId == userId && s.User != null && !s.User.IsDeleted)
                .Include(s => s.User)
                    .ThenInclude(u => u.UserProfile)
                .Include(s => s.Parent)
                    .ThenInclude(p => p.User)
                        .ThenInclude(u => u.UserProfile)
                .FirstOrDefaultAsync();
        }

        public async Task<Student> GetStudentByIdAsync(int studentId)
        {
            return await _context.Students
                .Where(s => !s.IsDeleted && s.StudentId == studentId && s.User != null && !s.User.IsDeleted)
                .Include(s => s.User)
                    .ThenInclude(u => u.UserProfile)
                .Include(s => s.Parent)
                    .ThenInclude(p => p.User)
                        .ThenInclude(u => u.UserProfile)
                .FirstOrDefaultAsync();
        }
    }
}
