using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Domain.DTOs
{
    public class ScheduleRequestDto
    {
        public int CourseId { get; set; }
        public int TeacherId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public byte MaxStudents { get; set; }
        public bool IsRecurring { get; set; }
        public string? RecurrenceRule { get; set; }
        public string? MeetingLink { get; set; } = string.Empty;
    }

    public class ScheduleResponseDto
    {
        public int ScheduleId { get; set; }
        public int CourseId { get; set; }
        public int TeacherId { get; set; }
        public int StudentId { get; set; } 
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public byte MaxStudents { get; set; }
        public byte CurrentStudents { get; set; }
        public string? MeetingLink { get; set; }
        public string Status { get; set; }
        public bool IsRecurring { get; set; }
        public string? RecurrenceRule { get; set; }
        public DateTime CreatedAt { get; set; }

        // Thông tin bổ sung
        public string? CourseTitle { get; set; }
        public string? TeacherName { get; set; }
        public string? StudentName { get; set; }
    }
}
