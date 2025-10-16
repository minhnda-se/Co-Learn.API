using CoLearn.Domain.Common;
using CoLearn.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using static CoLearn.Domain.DTOs.EnrollmentDtos;

namespace CoLearn.Domain.Interfaces.Repositories
{
    public interface IEnrollmentRepository : IGenericRepository<Enrollment>
    {
        Task<Enrollment?> GetByIdAsync(int id);

        Task<PagedResult<Enrollment>> GetAllEnrollmentsAsync(int pageIndex, int pageSize);

        Task<PagedResult<Enrollment>> GetByStudentIdAsync(int studentId, int pageIndex, int pageSize);

        Task<PagedResult<Enrollment>> GetByCourseIdAsync(int courseId, int pageIndex, int pageSize);
    }
}
