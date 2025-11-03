using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Domain.DTOs
{
    /// <summary>
    /// Dữ liệu biểu đồ xu hướng doanh thu
    /// </summary>
    public class RevenueTrendDto
    {
        public string TimeRange { get; set; } = string.Empty;
        public decimal TotalRevenue { get; set; }
        public List<TrendPointDto> Trend { get; set; } = new();
    }
    public class RevenueReportItem
    {
        public int Year { get; set; }
        public int? Month { get; set; } // Dùng nullable
        public int? Day { get; set; }   // Dùng nullable
        public decimal Revenue { get; set; }
    }

    /// <summary>
    /// Điểm dữ liệu theo giai đoạn (ngày/tháng/năm)
    /// </summary>
    public class TrendPointDto
    {
        public string Period { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
    }
    /// <summary>
    /// Báo cáo top giáo viên có doanh thu cao nhất
    /// </summary>
    public class TopTeachersReportDto
    {
        public int TotalTeachers { get; set; }
        public List<TopTeacherDto> TopTeachers { get; set; } = new();
    }

    /// <summary>
    /// Dữ liệu chi tiết của một giáo viên trong bảng xếp hạng
    /// </summary>
    public class TopTeacherDto
    {
        public int TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public decimal TotalEarnings { get; set; }
    }
}
