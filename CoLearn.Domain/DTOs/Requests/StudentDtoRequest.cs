using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Domain.DTOs.Requests
{
    public class StudentDtoRequest
    {
        /// <summary>
        /// Id của Parent nếu có (có thể null)
        /// </summary>
        public int? ParentId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime? Born { get; set; }
        public string Photo { get; set; }
        /// <summary>
        /// Trình độ lớp/khối (ví dụ: "Grade 5", "High School", ...)
        /// </summary>
        public string? GradeLevel { get; set; }
    }
}
