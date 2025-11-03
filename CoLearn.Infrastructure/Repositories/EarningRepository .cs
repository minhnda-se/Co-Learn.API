using CoLearn.Domain.DTOs;
using CoLearn.Domain.Enums;
using CoLearn.Domain.Interfaces.Repositories;
using CoLearn.Domain.Interfaces.Services;
using CoLearn.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Infrastructure.Repositories
{
    public class EarningRepository : IEarningRepository
    {
        protected readonly AppDbContext _context;
        private readonly decimal systemFeePercentage = 20; // Giả sử phí hệ thống là 20%

        public EarningRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TeacherEarningsDto?> GetTeacherEarningsByTeacherIdAsync(int teacherId, DateTime? startDate = null, DateTime? endDate = null)
        {
            // 1️⃣ Payments từ Booking (meeting)
            var bookingPayments = _context.Payments
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Schedules)
                        .ThenInclude(s => s.Course)
                .Include(p => p.Booking.Student)
                    .ThenInclude(s => s.User)
                .Where(p => p.Booking != null &&
                            p.Transactions.Count > 0 &&
                            p.Booking.TeacherId == teacherId &&
                            p.StatusId == (int)StatusEnum.Success &&
                            !p.IsDeleted);

            // 2️⃣ Payments từ Enrollment (course tự học)
            var enrollmentPayments = _context.Payments
                .Include(p => p.Enrollment)
                    .ThenInclude(e => e.Course)
                .Where(p => p.Enrollment != null &&
                            p.Transactions.Count > 0 &&
                            p.Enrollment.Course.TeacherId == teacherId &&
                            p.StatusId == (int)StatusEnum.Success &&
                            !p.IsDeleted);

            // 3️⃣ Filter theo thời gian
            if (startDate.HasValue)
            {
                bookingPayments = bookingPayments.Where(p => p.CreatedAt >= startDate);
                enrollmentPayments = enrollmentPayments.Where(p => p.CreatedAt >= startDate);
            }
            if (endDate.HasValue)
            {
                bookingPayments = bookingPayments.Where(p => p.CreatedAt <= endDate);
                enrollmentPayments = enrollmentPayments.Where(p => p.CreatedAt <= endDate);
            }

            // 4️⃣ Tổng doanh thu
            var totalFromBookings = await bookingPayments.SumAsync(p => (decimal?)p.Amount) ?? 0;
            var totalFromEnrollments = await enrollmentPayments.SumAsync(p => (decimal?)p.Amount) ?? 0;
            var totalEarnings = totalFromBookings + totalFromEnrollments;
            var totalTransactions = await bookingPayments.CountAsync() + await enrollmentPayments.CountAsync();

            // 5️⃣ COURSE BREAKDOWN (chỉ lấy từ enrollment)
            var courseBreakdown = await enrollmentPayments
                .Where(p => p.Enrollment != null && p.Enrollment.Course != null)
                .Select(p => new
                {
                    CourseId = p.Enrollment.Course.CourseId,
                    CourseTitle = p.Enrollment.Course.Title,
                    Amount = p.Amount
                })
                .GroupBy(x => new { x.CourseId, x.CourseTitle })
                .Select(g => new CourseEarningsDto
                {
                    CourseId = g.Key.CourseId,
                    CourseTitle = g.Key.CourseTitle,
                    CourseRevenue = g.Sum(x => x.Amount),
                    TotalBookings = g.Count()
                })
                .ToListAsync();

            // 6️⃣ BOOKING BREAKDOWN (chỉ lấy meeting)
            var bookingBreakdown = await bookingPayments
                .Select(p => new BookingEarningsDto
                {
                    BookingId = p.Booking.BookingId,
                    StudentName = p.Booking.Student.User.FullName,
                    Note = p.Booking.Notes,
                    BookingDate = p.Booking.CreatedAt,
                    Amount = p.Amount,
                    PaymentStatus = p.Status.StatusName
                })
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            // 7️⃣ Thông tin giáo viên
            var teacherName = await _context.Teachers
                .Include(t => t.User)
                .Where(t => t.TeacherId == teacherId)
                .Select(t => t.User.FullName)
                .FirstOrDefaultAsync() ?? "Unknown";

            // 8️⃣ Trả về DTO
            return new TeacherEarningsDto
            {
                TeacherId = teacherId,
                TeacherName = teacherName,
                TotalEarnings = totalEarnings,
                TotalFromBookings = totalFromBookings,
                TotalFromEnrollments = totalFromEnrollments,
                SystemFees = totalEarnings * systemFeePercentage / 100,
                TotalRevenue = totalEarnings - (totalEarnings * systemFeePercentage / 100),
                TotalTransactions = totalTransactions,
                CourseBreakdown = courseBreakdown,
                BookingBreakdown = bookingBreakdown
            };
        }

        public async Task<List<TeacherEarningsDto>> GetAllTeacherEarningsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            // Lấy danh sách tất cả teacher có doanh thu
            var teachers = await _context.Teachers
                .Include(t => t.User)
                .Select(t => new { t.TeacherId, t.User.FullName })
                .ToListAsync();

            var result = new List<TeacherEarningsDto>();

            foreach (var teacher in teachers)
            {
                // 1️⃣ Payments từ Booking
                var bookingPayments = _context.Payments
                    .Include(p => p.Booking)
                        .ThenInclude(b => b.Schedules)
                            .ThenInclude(s => s.Course)
                    .Include(p => p.Booking.Student)
                        .ThenInclude(s => s.User)
                    .Where(p => p.Booking != null &&
                                p.Transactions.Count > 0 &&
                                p.Booking.TeacherId == teacher.TeacherId &&
                                p.StatusId == (int)StatusEnum.Success &&
                                !p.IsDeleted);

                // 2️⃣ Payments từ Enrollment
                var enrollmentPayments = _context.Payments
                    .Include(p => p.Enrollment)
                        .ThenInclude(e => e.Course)
                    .Where(p => p.Enrollment != null &&
                                p.Transactions.Count > 0 &&
                                p.Enrollment.Course.TeacherId == teacher.TeacherId &&
                                p.StatusId == (int)StatusEnum.Success &&
                                !p.IsDeleted);

                // 3️⃣ Filter thời gian
                if (startDate.HasValue)
                {
                    bookingPayments = bookingPayments.Where(p => p.CreatedAt >= startDate);
                    enrollmentPayments = enrollmentPayments.Where(p => p.CreatedAt >= startDate);
                }
                if (endDate.HasValue)
                {
                    bookingPayments = bookingPayments.Where(p => p.CreatedAt <= endDate);
                    enrollmentPayments = enrollmentPayments.Where(p => p.CreatedAt <= endDate);
                }

                // 4️⃣ Tổng doanh thu
                var totalFromBookings = await bookingPayments.SumAsync(p => (decimal?)p.Amount) ?? 0;
                var totalFromEnrollments = await enrollmentPayments.SumAsync(p => (decimal?)p.Amount) ?? 0;
                var totalEarnings = totalFromBookings + totalFromEnrollments;
                var totalTransactions = await bookingPayments.CountAsync() + await enrollmentPayments.CountAsync();

                // 5️⃣ Course Breakdown
                var courseBreakdown = await enrollmentPayments
                    .Where(p => p.Enrollment != null && p.Enrollment.Course != null)
                    .Select(p => new
                    {
                        CourseId = p.Enrollment.Course.CourseId,
                        CourseTitle = p.Enrollment.Course.Title,
                        Amount = p.Amount
                    })
                    .GroupBy(x => new { x.CourseId, x.CourseTitle })
                    .Select(g => new CourseEarningsDto
                    {
                        CourseId = g.Key.CourseId,
                        CourseTitle = g.Key.CourseTitle,
                        CourseRevenue = g.Sum(x => x.Amount),
                        TotalBookings = g.Count()
                    })
                    .ToListAsync();

                // 6️⃣ Booking Breakdown
                var bookingBreakdown = await bookingPayments
                    .Select(p => new BookingEarningsDto
                    {
                        BookingId = p.Booking.BookingId,
                        StudentName = p.Booking.Student.User.FullName,
                        Note = p.Booking.Notes,
                        BookingDate = p.Booking.CreatedAt,
                        Amount = p.Amount,
                        PaymentStatus = p.Status.StatusName
                    })
                    .OrderByDescending(b => b.BookingDate)
                    .ToListAsync();

                // 7️⃣ Add teacher vào danh sách kết quả
                result.Add(new TeacherEarningsDto
                {
                    TeacherId = teacher.TeacherId,
                    TeacherName = teacher.FullName,
                    TotalEarnings = totalEarnings,
                    TotalFromBookings = totalFromBookings,
                    TotalFromEnrollments = totalFromEnrollments,
                    SystemFees = totalEarnings * systemFeePercentage / 100,
                    TotalRevenue = totalEarnings - (totalEarnings * systemFeePercentage / 100),
                    TotalTransactions = totalTransactions,
                    CourseBreakdown = courseBreakdown,
                    BookingBreakdown = bookingBreakdown
                });
            }

            return result.OrderByDescending(t => t.TotalEarnings).ToList();
        }

        public async Task<RevenueTrendDto> GetTeacherRevenueTrendsAsync(
    int? teacherId = null,
    DateTime? startDate = null,
    DateTime? endDate = null,
    string groupBy = "month")
        {
            var payments = _context.Payments
                .Include(p => p.Booking)
                .Include(p => p.Enrollment)
                .Where(p => p.Transactions.Any() &&
                            p.StatusId == (int)StatusEnum.Success &&
                            !p.IsDeleted);

            if (teacherId.HasValue)
            {
                payments = payments.Where(p =>
                    (p.Booking != null && p.Booking.TeacherId == teacherId) ||
                    (p.Enrollment != null && p.Enrollment.Course.TeacherId == teacherId));
            }

            if (startDate.HasValue)
                payments = payments.Where(p => p.CreatedAt >= startDate);
            if (endDate.HasValue)
                payments = payments.Where(p => p.CreatedAt <= endDate);

            // 1️⃣ SỬA LỖI Ở ĐÂY:
            // Biến groupedData giờ đây sẽ có kiểu là List<RevenueReportItem>
            List<RevenueReportItem> groupedData = groupBy.ToLower() switch
            {
                "day" => await payments
                    .GroupBy(p => new { p.CreatedAt.Year, p.CreatedAt.Month, p.CreatedAt.Day })
                    .Select(g => new RevenueReportItem // <--- SỬA ĐỔI
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Day = g.Key.Day,
                        Revenue = g.Sum(x => x.Amount)
                    })
                    .ToListAsync(),

                "year" => await payments
                    .GroupBy(p => new { p.CreatedAt.Year })
                    .Select(g => new RevenueReportItem // <--- SỬA ĐỔI
                    {
                        Year = g.Key.Year,
                        Month = null, // Gán null cho giá trị bị thiếu
                        Day = null,   // Gán null cho giá trị bị thiếu
                        Revenue = g.Sum(x => x.Amount)
                    })
                    .ToListAsync(),

                _ => await payments // Default to month
                    .GroupBy(p => new { p.CreatedAt.Year, p.CreatedAt.Month })
                    .Select(g => new RevenueReportItem // <--- SỬA ĐỔI
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Day = null, // Gán null cho giá trị bị thiếu
                        Revenue = g.Sum(x => x.Amount)
                    })
                    .ToListAsync()
            };

            // 2️⃣ KHÔNG CẦN THAY ĐỔI:
            // Đoạn code này bây giờ sẽ hoạt động vì groupedData đã có kiểu
            // List<RevenueReportItem> và có đủ các thuộc tính Year, Month, Day.
            List<TrendPointDto> trend = groupedData.Select(g =>
            {
                string period = groupBy.ToLower() switch
                {
                    // Trình biên dịch sẽ dùng g.Month.Value và g.Day.Value
                    "day" => $"{g.Year:D4}-{g.Month:D2}-{g.Day:D2}",
                    "year" => $"{g.Year:D4}",
                    _ => $"{g.Year:D4}-{g.Month:D2}"
                };
                return new TrendPointDto
                {
                    Period = period,
                    Revenue = g.Revenue
                };
            })
            .OrderBy(x => x.Period)
            .ToList();

            return new RevenueTrendDto
            {
                TimeRange = $"{trend.FirstOrDefault()?.Period ?? ""} to {trend.LastOrDefault()?.Period ?? ""}",
                TotalRevenue = trend.Sum(t => t.Revenue),
                Trend = trend
            };
        }



        public async Task<TopTeachersReportDto> GetTopTeachersAsync(DateTime? startDate = null, DateTime? endDate = null, int limit = 10)
        {
            var payments = _context.Payments
                .Include(p => p.Booking)
                .Include(p => p.Enrollment)
                .Where(p => p.Transactions.Any() && p.StatusId == (int)StatusEnum.Success && !p.IsDeleted);

            if (startDate.HasValue)
                payments = payments.Where(p => p.CreatedAt >= startDate);
            if (endDate.HasValue)
                payments = payments.Where(p => p.CreatedAt <= endDate);

            var teacherEarnings = await payments
                .Select(p => new
                {
                    TeacherId = (p.Booking != null ? p.Booking.TeacherId : p.Enrollment.Course.TeacherId) ?? 0,
                    Amount = p.Amount
                })
                .GroupBy(x => x.TeacherId)
                .Select(g => new
                {
                    TeacherId = g.Key,
                    TotalEarnings = g.Sum(x => x.Amount)
                })
                .OrderByDescending(x => x.TotalEarnings)
                .Take(limit)
                .ToListAsync();

            var teacherIds = teacherEarnings.Select(t => t.TeacherId).ToList();

            var teacherNames = await _context.Teachers
                .Include(t => t.User)
                .Where(t => teacherIds.Contains(t.TeacherId))
                .ToDictionaryAsync(t => t.TeacherId, t => t.User.FullName);

            var result = teacherEarnings
                .Select(t => new TopTeacherDto
                {
                    TeacherId = t.TeacherId,
                    TeacherName = teacherNames.ContainsKey(t.TeacherId) ? teacherNames[t.TeacherId] : "Unknown",
                    TotalEarnings = t.TotalEarnings
                })
                .ToList();

            return new TopTeachersReportDto
            {
                TotalTeachers = result.Count(),
                TopTeachers = result
            };
        }
    }
}
