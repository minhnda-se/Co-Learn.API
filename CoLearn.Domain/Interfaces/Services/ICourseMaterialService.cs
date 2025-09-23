using CoLearn.Domain.Common;
using CoLearn.Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoLearn.Domain.Interfaces.Services
{
    public interface ICourseMaterialService
    {
        Task<int> CreateAsync(int lessonId, CourseMaterialRequestDto dto);
        Task<int> UpdateAsync(int id, CourseMaterialRequestDto dto);
        Task<int> DeleteAsync(int id);
        Task<Result<PagedResult<CourseMaterialResponseDto>>> GetByLessonIdAsync(int pageIndex, int pageSize, int lessonId);
        Task<Result<CourseMaterialResponseDto?>> GetByIdAsync(int id);
    }
}
