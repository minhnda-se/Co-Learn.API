using CoLearn.Domain.Interfaces.Repositories;
using CoLearn.Domain.Models;
using CoLearn.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoLearn.Infrastructure.Repositories
{
    public class CourseMaterialRepository : GenericRepository<CourseMaterial>, ICourseMaterialRepository
    {
        public CourseMaterialRepository(AppDbContext context) : base(context) { }

        public async Task<CourseMaterial?> GetByIdAsync(int id)
        {
            return await _context.CourseMaterials
                .Include(m => m.Lesson)
                .Include(m => m.Course)
                .FirstOrDefaultAsync(m => m.MaterialId == id && !m.IsDeleted);
        }

        public async Task<List<CourseMaterial>> GetByLessonIdAsync(int lessonId)
        {
            return await _context.CourseMaterials
                .Where(m => m.LessonId == lessonId && !m.IsDeleted)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }
    }
}
