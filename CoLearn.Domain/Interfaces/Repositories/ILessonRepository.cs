using CoLearn.Domain.Common;
using CoLearn.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoLearn.Domain.Interfaces.Repositories
{
    public interface ILessonRepository : IGenericRepository<Lesson>
    {
        Task<Lesson?> GetByIdAsync(int id);
        Task<PagedResult<Lesson>> GetByCourseIdAsync(int pageIndex, int pageSize,int courseId);
    }
}
