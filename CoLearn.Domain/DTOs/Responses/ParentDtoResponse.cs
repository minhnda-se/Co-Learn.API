using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Domain.DTOs.Responses
{
    public class ParentDtoResponse
    {
        public int ParentId { get; set; }
        public int UserId { get; set; }

        // Dữ liệu từ User
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Dữ liệu từ Parent
        public string? Relationship { get; set; }

        // Dữ liệu từ UserProfile (nếu có)
        public UserProfileDto? UserProfile { get; set; }
        public List<StudentDtoResponse> Children { get; set; } = new();
    }
}
