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
        Task<List<Course>> GetAllCourseAsync();
        Task<Course?> GetByIdAsync(int id);
        Task<List<Course>> SearchCoursesAsync(string? keyword, string? teacherName);
    }
}
