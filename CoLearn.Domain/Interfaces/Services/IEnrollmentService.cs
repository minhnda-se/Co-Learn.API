using CoLearn.Domain.Common;
using CoLearn.Domain.DTOs;
using System.Threading.Tasks;
using static CoLearn.Domain.DTOs.EnrollmentDtos;

namespace CoLearn.Domain.Interfaces.Services
{
    public interface IEnrollmentService
    {
        Task<Result<PagedResult<EnrollmentResponseDto>>> GetAllAsync(int pageIndex, int pageSize);
        Task<Result<PagedResult<EnrollmentResponseDto>>> GetByStudentIdAsync(int studentId, int pageIndex, int pageSize);
        Task<Result<PagedResult<EnrollmentResponseDto>>> GetByCourseIdAsync(int courseId, int pageIndex, int pageSize);
        Task<Result<EnrollmentResponseDto?>> GetByIdAsync(int enrollmentId);

        Task<int> CreateAsync(EnrollmentRequestDto dto);
        Task<int> UpdateAsync(int id, EnrollmentRequestDto dto);
        Task<int> DeleteAsync(int id);
    }
}
