using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Domain.DTOs
{
    public class EnrollmentDtos
    {
        public class EnrollmentRequestDto
        {
            public int StudentId { get; set; }
            public int CourseId { get; set; }
            public string Status { get; set; } = null!;
            public decimal ProgressPercent { get; set; }
        }

        public class EnrollmentResponseDto
        {
            public int EnrollmentId { get; set; }
            public int StudentId { get; set; }
            public int CourseId { get; set; }
            public DateTime EnrolledAt { get; set; }
            public string Status { get; set; } = null!;
            public decimal ProgressPercent { get; set; }
            public DateTime? UpdatedAt { get; set; }

            // Related data
            public string? StudentName { get; set; }
            public string? StudentEmail { get; set; }
            public CourseResponseDto? Course { get; set; }
        }
    }
}
