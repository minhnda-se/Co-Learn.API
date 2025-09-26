using CoLearn.Domain.Models;
using System;

namespace CoLearn.Domain.DTOs
{
    public class CourseRequestDto
    {
        public int? TeacherId { get; set; }
        public int? CategoryId { get; set; }
        public string Title { get; set; } = null!;
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public string? Level { get; set; }
        public decimal? PricePerSession { get; set; }
        public short? DurationMinutes { get; set; }
        public string? ImageUrl { get; set; }

    }

    public class CourseResponseDto
    {
        public int CourseId { get; set; }
        public string Title { get; set; } = null!;
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public string? Level { get; set; }
        public decimal? PricePerSession { get; set; }
        public short? DurationMinutes { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ImageUrl { get; set; }

        // Flatten
        public int? TeacherId { get; set; }
        public string? TeacherName { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }

        public List<LessonResponseDto>? Lessons { get; set; }
    }
}
