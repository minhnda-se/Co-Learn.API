using CoLearn.Domain.Common;
using CoLearn.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoLearn.Domain.Interfaces.Repositories
{
    public interface ICourseMaterialRepository : IGenericRepository<CourseMaterial>
    {
        Task<CourseMaterial?> GetByIdAsync(int id);
        Task<PagedResult<CourseMaterial>> GetByLessonIdAsync(int pageIndex, int pageSize, int lessonId);
    }
}
