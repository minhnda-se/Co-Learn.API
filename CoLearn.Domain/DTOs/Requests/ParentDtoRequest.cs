using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Domain.DTOs.Requests
{
    public class ParentDtoRequest
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime? Born { get; set; }
        public string Photo { get; set; }

        /// <summary>
        /// Quan hệ với học sinh (ví dụ: Father, Mother, Guardian, ...)
        /// </summary>
        public string? Relationship { get; set; }
    }
}
