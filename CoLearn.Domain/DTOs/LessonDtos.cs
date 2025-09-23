using CoLearn.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Domain.DTOs
{
    public class LessonRequestDto
    {
        public int CourseId { get; set; }
        public string Title { get; set; } = null!;
        public string? Content { get; set; }
        public byte? OrderNumber { get; set; }
        public short? DurationMinutes { get; set; }
    }

    public class LessonResponseDto
    {
        public int LessonId { get; set; }
        public int CourseId { get; set; }
        public string Title { get; set; } = null!;
        public string? Content { get; set; }
        public byte? OrderNumber { get; set; }
        public short? DurationMinutes { get; set; }
        public DateTime CreatedAt { get; set; }

        // Thông tin bổ sung
        public string? CourseTitle { get; set; }
        public List<CourseMaterialResponseDto>? CourseMaterials { get; set; }
    }

}
