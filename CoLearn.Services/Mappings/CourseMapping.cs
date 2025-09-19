using AutoMapper;
using CoLearn.Domain.DTOs;
using CoLearn.Domain.DTOs.Requests;
using CoLearn.Domain.DTOs.Responses;
using CoLearn.Domain.Models;

namespace CoLearn.Services.Mappings
{
    public class CourseMapping : Profile
    {
        public CourseMapping()
        {
            // Request -> Entity
            CreateMap<CourseRequestDto, Course>()
                .ForMember(dest => dest.CourseId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CourseMaterials, opt => opt.Ignore())
                .ForMember(dest => dest.Enrollments, opt => opt.Ignore())
                .ForMember(dest => dest.Lessons, opt => opt.Ignore())
                .ForMember(dest => dest.Schedules, opt => opt.Ignore())
                .ForMember(dest => dest.Teacher, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore());

            // Entity -> Response
            CreateMap<Course, CourseResponseDto>()
                .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teacher != null ? src.Teacher.User.FullName : null)) 
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null));
        }
    }
}
