using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Domain.DTOs.Requests
{
    public class TeacherDtoRequest
    {
        public int UserId { get; set; } 
        public string? Bio { get; set; }
        public string? Qualification { get; set; }
        public byte? YearsExperience { get; set; }
        public string? VerificationStatus { get; set; }
        public decimal? HourlyRate { get; set; }
        public decimal? AvgRating { get; set; }
    }
}
