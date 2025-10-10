namespace CoLearn.Domain.DTOs.Responses
{
    public class TeacherDtoResponse
    {
        public int TeacherId { get; set; }
        public int UserId { get; set; }

        // Thông tin cơ bản
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int? Age { get; set; }
        public DateTime? Born { get; set; }
        public string? Phone { get; set; }
        public string? Gender { get; set; }

        // Hồ sơ chuyên môn
        public string? Degree { get; set; }   // file path PDF
        public string? Cv { get; set; }       // file path PDF
        public string? Photo { get; set; }    // URL ảnh hoặc blob
        public string? Description { get; set; } // giới thiệu bản thân

        // Dữ liệu hệ thống (nếu cần)
        public decimal? HourlyRate { get; set; }
        public decimal? AvgRating { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
