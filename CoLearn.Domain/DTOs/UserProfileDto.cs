using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Domain.DTOs
{
    public class UserProfileDto
    {
        public int UserId { get; set; }   // foreign key, bắt buộc 
        public string? AvatarUrl { get; set; }
        public string? Bio { get; set; }
        public string? Address { get; set; }
        public string? ExtraJson { get; set; }
    }
}
