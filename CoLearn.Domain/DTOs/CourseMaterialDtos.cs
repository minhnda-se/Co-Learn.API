using System;

namespace CoLearn.Domain.DTOs
{
    // Request DTO: dùng cho tạo/sửa
    public class CourseMaterialRequestDto
    {
        public string? Title { get; set; }
        public string? MaterialType { get; set; } // file, video, link
        public string? Url { get; set; }
    }

    // Response DTO: trả về khi xem/list
    public class CourseMaterialResponseDto
    {
        public int MaterialId { get; set; }
        public int? LessonId { get; set; }
        public int? CourseId { get; set; }
        public string? Title { get; set; }
        public string? MaterialType { get; set; }
        public string? Url { get; set; }
        public DateTime CreatedAt { get; set; }

        // thông tin bổ sung
        public string? LessonTitle { get; set; }
        public string? CourseTitle { get; set; }
    }
}
