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

        private IQueryable<Enrollment> BuildEnrollmentQuery()
        {
            return _context.Enrollments
                .Include(e => e.Course)
                    .ThenInclude(c => c.Teacher)
                        .ThenInclude(t => t.User)
                .Include(e => e.Course)
                    .ThenInclude(c => c.Category)
                .Include(e => e.Student)
                    .ThenInclude(s => s.User)
                .Where(e => !e.IsDeleted);
        }

        public async Task<PagedResult<Enrollment>> GetByStudentIdAsync(int studentId, int pageIndex, int pageSize)
        {
            var query = BuildEnrollmentQuery()
                .Where(e => e.StudentId == studentId && !e.IsDeleted);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Enrollment>(items, pageIndex, pageSize, totalCount);
        }

        public async Task<PagedResult<Enrollment>> GetByCourseIdAsync(int courseId, int pageIndex, int pageSize)
        {
            var query = BuildEnrollmentQuery()
                .Where(e => e.CourseId == courseId && !e.IsDeleted);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Enrollment>(items, pageIndex, pageSize, totalCount);
        }

        public async Task<PagedResult<Enrollment>> GetAllEnrollmentsAsync(int pageIndex, int pageSize)
        {
            var query = BuildEnrollmentQuery().Where(d => !d.IsDeleted);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Enrollment>(items, pageIndex, pageSize, totalCount);
        }

        public async Task<Enrollment?> GetByIdAsync(int id)
        {
            return await BuildEnrollmentQuery()
                .FirstOrDefaultAsync(e => e.EnrollmentId == id && !e.IsDeleted);
        }
    }
}
