using AutoMapper;
using CoLearn.Domain.DTOs;
using CoLearn.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Services.Mappings
{
    public class LessonMapping : Profile
    {
        public LessonMapping()
        {
            // Request -> Entity
            CreateMap<LessonRequestDto, Lesson>()
                .ForMember(dest => dest.LessonId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Assignments, opt => opt.Ignore())
                .ForMember(dest => dest.Course, opt => opt.Ignore())
                .ForMember(dest => dest.CourseMaterials, opt => opt.Ignore())
                .ForMember(dest => dest.StudentProgresses, opt => opt.Ignore());

            // Entity -> Response
            CreateMap<Lesson, LessonResponseDto>()
                .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course != null ? src.Course.Title : null));
        }
    }
}