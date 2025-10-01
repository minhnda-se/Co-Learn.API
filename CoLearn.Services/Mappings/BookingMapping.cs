using AutoMapper;
using CoLearn.Domain.DTOs;
using CoLearn.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CoLearn.Domain.DTOs.BookingDtos;

namespace CoLearn.Services.Mappings
{
    class BookingMapping : Profile
    {
        public BookingMapping()
        {
            // Map RequestDto -> Entity
            CreateMap<BookingRequestDto, Booking>();

            // Map Entity -> ResponseDto
            CreateMap<Booking, BookingResponseDto>()
                .ForMember(dest => dest.StudentName,
                           opt => opt.MapFrom(src => src.Student != null ? src.Student.User.FullName : null))
                //.ForMember(dest => dest.ScheduleTitle,
                //           opt => opt.MapFrom(src => src.Schedule != null ? src.Schedule.Title : null))
                .ForMember(dest => dest.BookingStatusName,
                           opt => opt.MapFrom(src => src.BookingStatus != null ? src.BookingStatus.StatusName : null));

            CreateMap<Booking, BookingEmailDto>()
                .ForMember(dest => dest.TeacherName,
                           opt => opt.MapFrom(src => src.Teacher != null && src.Teacher.User != null
                               ? src.Teacher.User.FullName
                               : string.Empty))
                .ForMember(dest => dest.TeacherEmail,
                           opt => opt.MapFrom(src => src.Teacher != null && src.Teacher.User != null
                               ? src.Teacher.User.Email
                               : string.Empty))
                .ForMember(dest => dest.StudentName,
                           opt => opt.MapFrom(src => src.Student != null && src.Student.User != null
                               ? src.Student.User.FullName
                               : string.Empty))
                .ForMember(dest => dest.StudentEmail,
                           opt => opt.MapFrom(src => src.Student != null && src.Student.User != null
                               ? src.Student.User.Email
                               : string.Empty))
                .ForMember(dest => dest.CreateAt,
                           opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.StartTime,
                           opt => opt.MapFrom(src => src.RequestedStartTime ?? DateTime.MinValue))
                .ForMember(dest => dest.EndTime,
                           opt => opt.MapFrom(src => src.RequestedEndTime ?? DateTime.MinValue))
                .ForMember(dest => dest.Notes,
                           opt => opt.MapFrom(src => src.Notes ?? string.Empty));


        }
    }
}
