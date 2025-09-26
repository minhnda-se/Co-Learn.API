using CoLearn.Domain.Common;
using CoLearn.Domain.Interfaces.Repositories;
using CoLearn.Domain.Models;
using CoLearn.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoLearn.Infrastructure.Repositories
{
    public class EnrollmentRepository : GenericRepository<Enrollment>, IEnrollmentRepository
    {
        public EnrollmentRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<Enrollment>> GetByStudentIdAsync(int studentId, int pageIndex, int pageSize)
        {
            var query = _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                    .ThenInclude(s => s.User)
                .Where(e => e.StudentId == studentId && !e.IsDeleted)
                .AsQueryable();

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Enrollment>(items, pageIndex, pageSize, totalCount);
        }

        public async Task<PagedResult<Enrollment>> GetByCourseIdAsync(int courseId, int pageIndex, int pageSize)
        {
            var query = _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                    .ThenInclude(s => s.User)
                .Where(e => e.CourseId == courseId && !e.IsDeleted)
                .AsQueryable();

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Enrollment>(items, pageIndex, pageSize, totalCount);
        }

        public async Task<PagedResult<Enrollment>> GetAllEnrollmentsAsync(int pageIndex, int pageSize)
        {
            var query = _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                    .ThenInclude(s => s.User)
                .AsQueryable();

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Enrollment>(items, pageIndex, pageSize, totalCount);
        }

        public async Task<Enrollment?> GetByIdAsync(int id)
        {
            return await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                    .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(e => e.EnrollmentId == id && !e.IsDeleted);
        }
    }
}
