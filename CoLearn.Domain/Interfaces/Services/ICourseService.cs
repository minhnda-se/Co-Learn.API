using CoLearn.Domain.DTOs;
using CoLearn.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Domain.Interfaces.Services
{
    public interface ICourseService
    {
        Task<List<CourseResponseDto>> GetAllCourseAsync();
        Task<List<CourseResponseDto>> GetAllCourseByTeacherId(int teacherId);

        Task<List<CourseResponseDto>> SearchCoursesAsync(string? keyword, string? teacherName);
        Task<CourseResponseDto?> GetByIdAsync(int courseId);
        Task<int> CreateAsync(CourseRequestDto dto);
        Task<int> UpdateAsync(int id, CourseRequestDto dto);
        Task<int> DeleteAsync(int couseId);
        Task<int> SetCourseActive(int courseId, bool? isActice);

    }
}
