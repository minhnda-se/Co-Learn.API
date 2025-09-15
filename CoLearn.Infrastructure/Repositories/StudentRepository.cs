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
    public class StudentRepository : GenericRepository<Student>, IStudentRepository
    {
        public StudentRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<Student>> GetAllStudentsAsync()
        {
            return await _context.Students
                .Include(s => s.User).ThenInclude(s => s.UserProfile)
                .ToListAsync();
        }

        public async Task<Student> GetByUserIdAsync(int userId)
        {
            return await _context.Students
                .Include(s => s.User).ThenInclude(s => s.UserProfile)
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }

        public async Task<Student> GetStudentByIdAsync(int studentId)
        {
            return await _context.Students
                .Include(s => s.User).ThenInclude(s => s.UserProfile)
                .FirstOrDefaultAsync(s => s.StudentId == studentId);
        }
    }
}
