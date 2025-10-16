using CoLearn.Domain.Common;
using CoLearn.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Domain.Interfaces.Repositories
{
    public interface ICourseRepository : IGenericRepository<Course>
    {
        Task<List<Course>> GetAllCourseByTeacherId(int teacherId);

        Task<PagedResult<Course>> GetAllCourseAsync(int pageIndex, int pageSize);

        Task<Course?> GetByIdAsync(int id);
        Task<List<Course>> SearchCoursesAsync(string? keyword, string? teacherName);
        Task<int> SetCourseActive(int courseId, bool? isActice);
    }
}
