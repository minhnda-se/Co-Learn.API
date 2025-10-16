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
        // Thông tin cá nhân
        public string FullName { get; set; }
        public DateTime? Born { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }

        // Hồ sơ chuyên môn
        public string Degree { get; set; }
        public string Cv { get; set; }
        public string Photo { get; set; }

        // Giới thiệu
        public string Description { get; set; }
    }
}
