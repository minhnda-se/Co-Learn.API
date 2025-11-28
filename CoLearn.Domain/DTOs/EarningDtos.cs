using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Domain.DTOs
{
    public class TeacherEarningsDto
    {
        public int TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public decimal TotalEarnings { get; set; }
        public decimal TotalFromBookings { get; set; }
        public decimal TotalFromEnrollments { get; set; }
        public decimal SystemFees { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalTransactions { get; set; }
        public List<CourseEarningsDto>? CourseBreakdown { get; set; }
        public List<BookingEarningsDto>? BookingBreakdown { get; set; } // 👈 Thêm mới
    }

    public class CourseEarningsDto
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public decimal CourseRevenue { get; set; }
        public int TotalBookings { get; set; }
    }

    public class BookingEarningsDto
    {
        public int BookingId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
    }

}
