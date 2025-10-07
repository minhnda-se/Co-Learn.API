using System;

namespace CoLearn.Domain.DTOs
{
    public static class BookingDtos
    {
        public class BookingRequestDto
        {
            public int TeacherId { get; set; }
            public int StudentId { get; set; }
            public DateOnly Date { get; set; }         // yyyy-MM-dd
            public TimeOnly StartTime { get; set; }    // HH:mm
            public int DurationMinutes { get; set; }   // số phút (FE gửi int thay vì string)
            public string? Notes { get; set; }
            
        }

        public class BookingResponseDto
        {
            public int BookingId { get; set; }
            public int TeacherId { get; set; }  
            public int ScheduleId { get; set; }
            public int StudentId { get; set; }
            public int BookingStatusId { get; set; }
            public DateTime? RequestedStartTime { get; set; }
            public DateTime? RequestedEndTime { get; set; } 
            public string? Notes { get; set; }
            public bool IsPaid { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }

            // Thêm thông tin liên quan nếu muốn hiển thị ra ngoài
            public string? StudentName { get; set; }
            public string? StudentEmail { get; set; }
            public string? TeacherName { get; set; }
            public string? TeacherEmail { get; set; }
            public string? ParentName { get; set; }
            public string? ParentEmail { get; set; }
            //public string? ScheduleTitle { get; set; }
            public string? BookingStatusName { get; set; }

        }
    }
}
