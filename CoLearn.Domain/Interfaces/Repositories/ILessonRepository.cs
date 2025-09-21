using CoLearn.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoLearn.Domain.Interfaces.Repositories
{
    public interface ILessonRepository : IGenericRepository<Lesson>
    {
        Task<Lesson?> GetByIdAsync(int id);
        Task<List<Lesson>> GetByCourseIdAsync(int courseId);
    }
}
