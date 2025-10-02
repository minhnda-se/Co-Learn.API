using System;

namespace CoLearn.Domain.DTOs
{
    public static class BookingDtos
    {
        public class BookingRequestDto
        {
            public int TeacherId { get; set; }
            public int StudentId { get; set; }
            public int BookingStatusId { get; set; }
            public string? Notes { get; set; }
            public bool IsPaid { get; set; }
        }

        public class BookingResponseDto
        {
            public int BookingId { get; set; }
            public int TeacherId { get; set; }  
            public int ScheduleId { get; set; }
            public int StudentId { get; set; }
            public int BookingStatusId { get; set; }
            public string? Notes { get; set; }
            public bool IsPaid { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }

            // Thêm thông tin liên quan nếu muốn hiển thị ra ngoài
            public string? StudentName { get; set; }
            //public string? ScheduleTitle { get; set; }
            public string? BookingStatusName { get; set; }
        }
    }
}
