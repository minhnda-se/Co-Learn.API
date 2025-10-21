using AutoMapper;
using CoLearn.Domain.Common;
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
        private readonly IS3StorageService _s3StorageService; 

        public CourseService(IUnitOfWork unitOfWork, IMapper mapper, IS3StorageService s3StorageService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _s3StorageService = s3StorageService; 
        }


        public async Task<int> CreateAsync(CourseRequestDto dto)
        {
            var course = _mapper.Map<Course>(dto);
            course.CreatedAt = DateTime.UtcNow;
            course.IsActive = false;
            course.IsDeleted = false;

            // Nếu FE gửi ImageUrl từ temp/, chuyển sang private/
            if (!string.IsNullOrEmpty(dto.ImageUrl))
            {
                try
                {
                    // Lấy fileKey từ URL
                    var tempKey = dto.ImageUrl.Replace(_s3StorageService.GetBaseUrl(), "");

                    var newKey = tempKey.Replace("temp/", "private/");
                    course.ImageUrl = await _s3StorageService.MoveFileAsync(tempKey, newKey);
                }
                catch (Exception ex)
                {
                    // Log lỗi nhưng không ảnh hưởng đến tạo course
                    Console.WriteLine($"Lỗi move ảnh: {ex.Message}");
                }
            }

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

        public async Task<Result<CourseResponseDto?>> GetByIdAsync(int courseId)
        {
            var course = await _unitOfWork.CourseRepository.GetByIdAsync(courseId);
            if (course == null)
                return Result<CourseResponseDto?>.Failure("Course not found");

            var courseDto = _mapper.Map<CourseResponseDto>(course);
            return Result<CourseResponseDto?>.Success(courseDto);
        }


        public async Task<Result<PagedResult<CourseResponseDto>>> GetAllCourseAsync(int pageIndex, int pageSize)
        {
            var pagedCourses = await _unitOfWork.CourseRepository.GetAllCourseAsync(pageIndex, pageSize);

            if (pagedCourses.Items.Count == 0)
                return Result<PagedResult<CourseResponseDto>>.Failure("No courses found");

            var courseDtos = _mapper.Map<List<CourseResponseDto>>(pagedCourses.Items);

            var response = new PagedResult<CourseResponseDto>(
                courseDtos,
                pagedCourses.PageIndex,
                pagedCourses.PageSize,
                pagedCourses.TotalCount
            );

            return Result<PagedResult<CourseResponseDto>>.Success(response);
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
