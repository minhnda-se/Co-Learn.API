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

        public async Task<List<Lesson>> GetByCourseIdAsync(int courseId)
        {
            return await _context.Lessons
                .Where(l => l.CourseId == courseId && !l.IsDeleted)
                .OrderBy(l => l.OrderNumber) // sắp xếp theo thứ tự
                .ToListAsync();
        }
    }
}
