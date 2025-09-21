using CoLearn.Domain.DTOs;
using CoLearn.Domain.DTOs.Requests;
using CoLearn.Domain.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoLearn.Domain.Interfaces.Services
{
    public interface ILessonService
    {
        Task<int> CreateAsync(int courseId, LessonRequestDto dto);
        Task<int> UpdateAsync(int id, LessonRequestDto dto);
        Task<int> DeleteAsync(int id);
        Task<List<LessonResponseDto>> GetByCourseIdAsync(int courseId);
    }
}
