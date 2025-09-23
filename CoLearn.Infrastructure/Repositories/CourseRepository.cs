using CoLearn.Domain.Common;
using CoLearn.Domain.Interfaces.Repositories;
using CoLearn.Domain.Models;
using CoLearn.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoLearn.Infrastructure.Repositories
{
    public class CourseRepository : GenericRepository<Course>, ICourseRepository
    {
        public CourseRepository(AppDbContext context) : base(context) { }
        // Lấy tất cả Course (chưa bị xóa)
        public async Task<PagedResult<Course>> GetAllCourseAsync(int pageIndex, int pageSize)
        {
            var query = _context.Courses
                .Where(c => !c.IsDeleted)
                .Include(c => c.Teacher).ThenInclude(t => t.User)
                .Include(c => c.Category)

                .Include(c => c.Lessons).ThenInclude(l => l.CourseMaterials)

                .AsQueryable();

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)

                .ToListAsync();

            return new PagedResult<Course>(items, pageIndex, pageSize, totalCount);
        }


        public async Task<List<Course>> GetAllCourseByTeacherId(int teacherId)
        {
            return await _context.Courses
               .Where(c => !c.IsDeleted && c.TeacherId == teacherId)
               .Include(c => c.Teacher).ThenInclude(t => t.User)
               .Include(c => c.Category)
               .Include(c => c.Lessons).ThenInclude(l => l.CourseMaterials)
               .ToListAsync();
        }


        // Lấy Course theo Id
        public async Task<Course?> GetByIdAsync(int id)
        {
            return await _context.Courses
                .Where(c => !c.IsDeleted && c.CourseId == id)
                .Include(c => c.Teacher).ThenInclude(t => t.User).Include(c => c.Category)
                .Include(c => c.Lessons).ThenInclude(l => l.CourseMaterials)
                .FirstOrDefaultAsync();
        }

        // Tìm kiếm Course theo keyword hoặc teacherName (logic OR)
        public async Task<List<Course>> SearchCoursesAsync(string? keyword, string? teacherName)
        {
            var query = _context.Courses
                .Where(c => !c.IsDeleted)
                .Include(c => c.Teacher).ThenInclude(t => t.User)
                .Include(c => c.Category)
                .Include(c => c.Lessons).ThenInclude(l => l.CourseMaterials)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword) || !string.IsNullOrWhiteSpace(teacherName))
            {
                query = query.Where(c =>
                    (!string.IsNullOrWhiteSpace(keyword) &&
                        (c.Title.Contains(keyword) ||
                         (c.Description != null && c.Description.Contains(keyword)) ||
                         (c.ShortDescription != null && c.ShortDescription.Contains(keyword)))
                    )
                    ||
                    (!string.IsNullOrWhiteSpace(teacherName) &&
                        c.Teacher != null && c.Teacher.User.FullName.Contains(teacherName)
                    )
                );
            }

            return await query.ToListAsync();
        }

        public async Task<int> SetCourseActive(int courseId, bool? isActice)
        {
            return await _context.Courses
                .Where(c => c.CourseId == courseId && !c.IsDeleted)
                .ExecuteUpdateAsync(c => c.SetProperty(c => c.IsActive, isActice ?? true));
        }
    }
}
