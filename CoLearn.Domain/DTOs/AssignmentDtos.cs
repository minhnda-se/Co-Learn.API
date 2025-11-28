using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Domain.DTOs
{
    public class AssignmentRequestDto
    {
        public int LessonId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
    }

    // Dùng khi trả về client
    public class AssignmentResponseDto
    {
        public int AssignmentId { get; set; }
        public int LessonId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; }

        // Optional: kèm thông tin Lesson
        public string? LessonTitle { get; set; }

        // Optional: thống kê số lượng submissions
        public int SubmissionCount { get; set; }
    }
}
