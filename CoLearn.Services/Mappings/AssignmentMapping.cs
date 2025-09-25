using AutoMapper;
using CoLearn.Domain.DTOs;
using CoLearn.Domain.DTOs.Responses;
using CoLearn.Domain.Models;
using System.Linq;

namespace CoLearn.Services.Mappings
{
    public class AssignmentMapping : Profile
    {
        public AssignmentMapping()
        {
            // Request -> Entity
            CreateMap<AssignmentRequestDto, Assignment>()
                .ForMember(dest => dest.AssignmentId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Lesson, opt => opt.Ignore())
                .ForMember(dest => dest.Submissions, opt => opt.Ignore());

            // Entity -> Response
            CreateMap<Assignment, AssignmentResponseDto>()
                .ForMember(dest => dest.LessonTitle, opt => opt.MapFrom(src => src.Lesson != null ? src.Lesson.Title : null))
                .ForMember(dest => dest.SubmissionCount, opt => opt.MapFrom(src => src.Submissions != null ? src.Submissions.Count : 0));

            CreateMap<Submission, SubmissionResponseDto>()
                // Student
                .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.StudentId))
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src =>
                    src.Student != null && src.Student.User != null
                        ? src.Student.User.FullName
                        : null))
                .ForMember(dest => dest.StudentEmail, opt => opt.MapFrom(src =>
                    src.Student != null && src.Student.User != null
                        ? src.Student.User.Email
                        : null))

                // Assignment
                .ForMember(dest => dest.AssignmentId, opt => opt.MapFrom(src => src.AssignmentId))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src =>
                    src.Assignment != null
                        ? src.Assignment.Title
                        : null))
                .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src =>
                    src.Assignment != null
                        ? src.Assignment.DueDate
                        : null));

            CreateMap<SubmissionRequestDto, Submission>()
                .ForMember(dest => dest.SubmittedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));
        }
    }
}
