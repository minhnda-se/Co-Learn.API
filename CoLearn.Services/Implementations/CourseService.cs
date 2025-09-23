using AutoMapper;
using CoLearn.Domain.DTOs;
using CoLearn.Domain.DTOs.Requests;
using CoLearn.Domain.DTOs.Responses;
using CoLearn.Domain.Interfaces;
using CoLearn.Domain.Interfaces.Services;
using CoLearn.Domain.Models;

namespace CoLearn.Services.Implementations
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CourseService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<int> CreateAsync(CourseRequestDto dto)
        {
            var course = _mapper.Map<Course>(dto);
            course.CreatedAt = DateTime.UtcNow;
            course.IsDeleted = false;
            course.IsDeleted = false;

            await _unitOfWork.CourseRepository.AddAndSaveAsync(course);
            await _unitOfWork.CommitAsync();

            return course.CourseId;
        }

        public async Task<int> UpdateAsync(int id, CourseRequestDto dto)
        {
            var course = await _unitOfWork.CourseRepository.GetByIdAsync(id);
            if (course == null) return 0;

            _mapper.Map(dto, course);
            await _unitOfWork.CourseRepository.UpdateAndSaveAsync(course);
            await _unitOfWork.CommitAsync();

            return course.CourseId;
        }

        public async Task<int> DeleteAsync(int courseId)
        {
            var course = await _unitOfWork.CourseRepository.GetByIdAsync(courseId);
            if (course == null) return 0;

            course.IsDeleted = true;
            course.DeletedAt = DateTime.UtcNow;

            await _unitOfWork.CourseRepository.UpdateAndSaveAsync(course);
            await _unitOfWork.CommitAsync();

            return course.CourseId;
        }

        public async Task<CourseResponseDto?> GetByIdAsync(int courseId)
        {
            var course = await _unitOfWork.CourseRepository.GetByIdAsync(courseId);
            return course == null ? null : _mapper.Map<CourseResponseDto>(course);
        }

        public async Task<List<CourseResponseDto>> GetAllCourseAsync()
        {
            var courses = await _unitOfWork.CourseRepository.GetAllAsync();
            return _mapper.Map<List<CourseResponseDto>>(courses);
        }

        public async Task<List<CourseResponseDto>> SearchCoursesAsync(string? keyword, string? teacherName)
        {
            var courses = await _unitOfWork.CourseRepository.SearchCoursesAsync(keyword, teacherName);
            return _mapper.Map<List<CourseResponseDto>>(courses);
        }

        public Task<int> SetCourseActive(int courseId, bool? isActice)
        {
            return _unitOfWork.CourseRepository.SetCourseActive(courseId, isActice);
        }

        public async Task<List<CourseResponseDto>> GetAllCourseByTeacherId(int teacherId)
        {
            var courses = await _unitOfWork.CourseRepository.GetAllCourseByTeacherId(teacherId);
            return _mapper.Map<List<CourseResponseDto>>(courses);
        }
    }
}
