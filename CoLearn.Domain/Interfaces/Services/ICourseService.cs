using CoLearn.Domain.Common;
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


        Task<Result<PagedResult<CourseResponseDto>>> GetAllCourseAsync(int pageIndex, int pageSize);

        Task<List<CourseResponseDto>> SearchCoursesAsync(string? keyword, string? teacherName);
        Task<Result<CourseResponseDto?>> GetByIdAsync(int courseId);
        Task<int> CreateAsync(CourseRequestDto dto);
        Task<int> UpdateAsync(int id, CourseRequestDto dto);
        Task<int> DeleteAsync(int couseId);
        Task<int> SetCourseActive(int courseId, bool? isActice);

    }
}
