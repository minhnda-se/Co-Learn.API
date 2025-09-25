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
    public class LessonRepository : GenericRepository<Lesson>, ILessonRepository
    {
        public LessonRepository(AppDbContext context) : base(context){}

        public async Task<Lesson?> GetByIdAsync(int id)
        {
            return await _context.Lessons
                .Include(l => l.Course)
                .Include(l => l.Assignments)
                .Include(l => l.CourseMaterials)
                .FirstOrDefaultAsync(l => l.LessonId == id && !l.IsDeleted);
        }

        public async Task<PagedResult<Lesson>> GetByCourseIdAsync(int pageIndex, int pageSize, int courseId)
        {
            var query = _context.Lessons
                .Where(l => l.CourseId == courseId && !l.IsDeleted)

                .Include(l => l.Course)
                .Include(l => l.Assignments)
                .Include(l => l.CourseMaterials)
                .OrderBy(l => l.OrderNumber) // sắp xếp theo thứ tự

                .OrderBy(l => l.OrderNumber);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)

                .ToListAsync();

            return new PagedResult<Lesson>(items, pageIndex, pageSize, totalCount);
        }
    }
}
