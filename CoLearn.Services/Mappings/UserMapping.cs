using AutoMapper;
using CoLearn.Domain.DTOs;
using CoLearn.Domain.DTOs.Requests;
using CoLearn.Domain.DTOs.Responses;
using CoLearn.Domain.Models;

namespace CoLearn.Services.Mappings
{
    internal class UserMapping : Profile
    {
        public UserMapping()
        {
            // ============================
            // UserProfile
            // ============================
            CreateMap<UserProfileDto, UserProfile>()
                .ForMember(dest => dest.ProfileId, opt => opt.Ignore()); // PK

            CreateMap<UserProfile, UserProfileDto>();

            // ============================
            // Teacher
            // ============================
            CreateMap<TeacherDtoRequest, Teacher>()
                .ForMember(dest => dest.TeacherId, opt => opt.Ignore()) // PK
                .ForMember(dest => dest.User, opt => opt.Ignore());     // Navigation

            CreateMap<Teacher, TeacherDtoResponse>()
                .ForMember(dest => dest.FullName,
                           opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.Email,
                           opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.UserProfile,
                           opt => opt.MapFrom(src => src.User.UserProfile));

            // ============================
            // Parent
            // ============================
            CreateMap<ParentDtoRequest, Parent>()
                .ForMember(dest => dest.ParentId, opt => opt.Ignore())  // PK
                .ForMember(dest => dest.User, opt => opt.Ignore());     // Navigation

            CreateMap<Parent, ParentDtoResponse>()
                .ForMember(dest => dest.FullName,
                           opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.Email,
                           opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.UserProfile,
                           opt => opt.MapFrom(src => src.User.UserProfile))
                .ForMember(dest => dest.Children,
                           opt => opt.MapFrom(src => src.Students));

            // ============================
            // Student
            // ============================
            CreateMap<StudentDtoRequest, Student>()
                .ForMember(dest => dest.StudentId, opt => opt.Ignore()) // PK
                .ForMember(dest => dest.User, opt => opt.Ignore())      // Navigation
                .ForMember(dest => dest.Parent, opt => opt.Ignore());   // Navigation

            CreateMap<Student, StudentDtoResponse>()
                .ForMember(dest => dest.FullName,
                           opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.Email,
                           opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.UserProfile,
                           opt => opt.MapFrom(src => src.User.UserProfile));
        }
    }
}
