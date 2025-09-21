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
        public async Task<List<Course>> GetAllCourseAsync()
        {
            return await _context.Courses
                .Where(c => !c.IsDeleted)
                .Include(c => c.Teacher)
                .Include(c => c.Category)
                .ToListAsync();
        }

        // Lấy Course theo Id
        public async Task<Course?> GetByIdAsync(int id)
        {
            return await _context.Courses
                .Where(c => !c.IsDeleted && c.CourseId == id)
                .Include(c => c.Teacher)
                .Include(c => c.Category)
                .FirstOrDefaultAsync();
        }

        // Tìm kiếm Course theo keyword hoặc teacherName (logic OR)
        public async Task<List<Course>> SearchCoursesAsync(string? keyword, string? teacherName)
        {
            var query = _context.Courses
                .Where(c => !c.IsDeleted)
                .Include(c => c.Teacher)
                .Include(c => c.Category)
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
