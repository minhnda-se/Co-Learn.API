using AutoMapper;
using CoLearn.Domain.DTOs.Requests;
using CoLearn.Domain.DTOs.Responses;
using CoLearn.Domain.Models;
using static CoLearn.Domain.DTOs.EnrollmentDtos;

namespace CoLearn.Services.Mappings
{
    public class EnrollmentMapping : Profile
    {
        public EnrollmentMapping()
        {
            // Request -> Entity
            CreateMap<EnrollmentRequestDto, Enrollment>();

            // Entity -> Response
            CreateMap<Enrollment, EnrollmentResponseDto>()
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src =>
                    src.Student != null && src.Student.User != null
                        ? src.Student.User.FullName
                        : null))
                .ForMember(dest => dest.StudentEmail, opt => opt.MapFrom(src =>
                    src.Student != null && src.Student.User != null
                        ? src.Student.User.Email
                        : null))
                .ForMember(dest => dest.Course, opt => opt.MapFrom(src => src.Course));
        }
    }
}
