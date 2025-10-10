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
                 .ForMember(dest => dest.User, opt => opt.Ignore())      // Navigation
                 .ForMember(dest => dest.Qualification,
                            opt => opt.MapFrom(src => $"{src.Degree}|{src.Cv}")) // gộp Degree + Cv vào Qualification
                 .ForMember(dest => dest.Bio,
                            opt => opt.MapFrom(src => src.Description));

            // Entity → Response
            CreateMap<Teacher, TeacherDtoResponse>()
                .ForMember(dest => dest.FullName,
                           opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.Email,
                           opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Born,
                           opt => opt.MapFrom(src => src.User.DateOfBirth))
                .ForMember(dest => dest.Phone,
                           opt => opt.MapFrom(src => src.User.Phone))
                .ForMember(dest => dest.Gender,
                           opt => opt.MapFrom(src => src.User.Gender))
                .ForMember(dest => dest.Age,
                           opt => opt.MapFrom(src =>
                               src.User.DateOfBirth.HasValue
                                   ? (int?)((DateTime.Now - src.User.DateOfBirth.Value).TotalDays / 365)
                                   : null))
                .ForMember(dest => dest.Photo,
                           opt => opt.MapFrom(src => src.User.UserProfile.AvatarUrl))
                .ForMember(dest => dest.Degree,
                           opt => opt.MapFrom(src => GetDegree(src.Qualification)))
                .ForMember(dest => dest.Cv,
                           opt => opt.MapFrom(src => GetCv(src.Qualification)))
                .ForMember(dest => dest.Description,
                           opt => opt.MapFrom(src => src.Bio));
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
                .ForMember(dest => dest.Born,
                           opt => opt.MapFrom(src => src.User.DateOfBirth))
                .ForMember(dest => dest.Phone,
                           opt => opt.MapFrom(src => src.User.Phone))
                .ForMember(dest => dest.Gender,
                           opt => opt.MapFrom(src => src.User.Gender))
                .ForMember(dest => dest.Age,
                           opt => opt.MapFrom(src =>
                               src.User.DateOfBirth.HasValue
                                   ? (int?)((DateTime.Now - src.User.DateOfBirth.Value).TotalDays / 365)
                                   : null))
                .ForMember(dest => dest.Photo,
                           opt => opt.MapFrom(src => src.User.UserProfile.AvatarUrl))
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
                 .ForMember(dest => dest.Born,
                            opt => opt.MapFrom(src => src.User.DateOfBirth))
                 .ForMember(dest => dest.Phone,
                            opt => opt.MapFrom(src => src.User.Phone))
                 .ForMember(dest => dest.Gender,
                            opt => opt.MapFrom(src => src.User.Gender))
                 .ForMember(dest => dest.Age,
                            opt => opt.MapFrom(src =>
                                src.User.DateOfBirth.HasValue
                                    ? (int?)((DateTime.Now - src.User.DateOfBirth.Value).TotalDays / 365)
                                    : null))
                 .ForMember(dest => dest.Photo,
                            opt => opt.MapFrom(src => src.User.UserProfile.AvatarUrl))
                .ForMember(dest => dest.UserProfile,
                           opt => opt.MapFrom(src => src.User.UserProfile));
        }

        private static string? GetDegree(string? qualification)
        {
            if (string.IsNullOrEmpty(qualification)) return null;
            var parts = qualification.Split('|');
            return parts.Length > 0 ? parts[0] : null;
        }

        private static string? GetCv(string? qualification)
        {
            if (string.IsNullOrEmpty(qualification)) return null;
            var parts = qualification.Split('|');
            return parts.Length > 1 ? parts[1] : null;
        }
    }
}
