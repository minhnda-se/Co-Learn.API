using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Domain.DTOs.Responses
{
    public class TeacherDtoResponse
    {
        public int TeacherId { get; set; }
        public int UserId { get; set; }

        // Dữ liệu lấy từ User
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Dữ liệu lấy từ Teacher
        public string? Bio { get; set; }
        public string? Qualification { get; set; }
        public byte? YearsExperience { get; set; }
        public string? VerificationStatus { get; set; }
        public decimal? HourlyRate { get; set; }
        public decimal? AvgRating { get; set; }

        // Dữ liệu lấy từ UserProfile
        public UserProfileDto? UserProfile { get; set; }
    }
}
