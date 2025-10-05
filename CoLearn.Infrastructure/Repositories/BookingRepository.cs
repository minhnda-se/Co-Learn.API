using CoLearn.Domain.Interfaces.Repositories;
using CoLearn.Domain.Models;
using CoLearn.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoLearn.Infrastructure.Repositories
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        public BookingRepository(AppDbContext context) : base(context) { }

        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.Student).ThenInclude(s => s.User)
                .Include(b => b.Student).ThenInclude(s => s.Parent).ThenInclude(p => p.User)
                .Include(b => b.Schedule)
                .Include(s => s.Teacher).ThenInclude(t => t.User)
                .Include(b => b.BookingStatus)
                .Include(b => b.Payments)
                .FirstOrDefaultAsync(b => b.BookingId == id && !b.IsDeleted);
        }

        public async Task<List<Booking>> GetAllBookingsAsync()
        {
            return await _context.Bookings
                .Include(b => b.Student).ThenInclude(s => s.User)
                .Include(b => b.Student).ThenInclude(s => s.Parent).ThenInclude(p => p.User)
                .Include(b => b.Schedule)
                .Include(s => s.Teacher).ThenInclude(t => t.User)
                .Include(b => b.BookingStatus)
                .Include(b => b.Payments)
                .Where(b => !b.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetByStudentIdAsync(int studentId)
        {
            return await _context.Bookings
                .Include(b => b.Student).ThenInclude(s => s.User)
                .Include(b => b.Student).ThenInclude(s => s.Parent).ThenInclude(p => p.User)
                .Include(b => b.Schedule)
                .Include(s => s.Teacher).ThenInclude(t => t.User)
                .Include(b => b.BookingStatus)
                .Include(b => b.Payments)
                .Where(b => b.StudentId == studentId && !b.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetByParentIdAsync(int parentId)
        {
            return await _context.Bookings
                .Include(b => b.Student).ThenInclude(s => s.User)
                .Include(b => b.Student).ThenInclude(s => s.Parent).ThenInclude(p => p.User)
                .Include(b => b.Schedule)
                .Include(s => s.Teacher).ThenInclude(t => t.User)
                .Include(b => b.BookingStatus)
                .Include(b => b.Payments)
                .Where(b => b.Student.Parent.ParentId == parentId && !b.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetByScheduleIdAsync(int scheduleId)
        {
            return await _context.Bookings
                .Include(b => b.Student).ThenInclude(s => s.User)
                .Include(b => b.Student).ThenInclude(s => s.Parent).ThenInclude(p => p.User)
                .Include(b => b.Schedule)
                .Include(s => s.Teacher).ThenInclude(t => t.User)
                .Include(b => b.BookingStatus)
                .Include(b => b.Payments)
                .Where(b => b.ScheduleId == scheduleId && !b.IsDeleted)
                .ToListAsync();
        }
        public async Task<List<Booking>> GetByTeacherIdAsync(int teacherId)
        {
            return await _context.Bookings
                .Include(b => b.Student).ThenInclude(s => s.User)
                .Include(b => b.Student).ThenInclude(s => s.Parent).ThenInclude(p => p.User)
                .Include(b => b.Schedule)
                .Include(s => s.Teacher).ThenInclude(t => t.User)
                .Include(b => b.BookingStatus)
                .Include(b => b.Payments)
                .Where(b => b.TeacherId == teacherId && !b.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetByStatusIdAsync(int statusId)
        {
            return await _context.Bookings
                .Include(b => b.Student).ThenInclude(s => s.User)
                .Include(b => b.Student).ThenInclude(s => s.Parent).ThenInclude(p => p.User)
                .Include(b => b.Schedule)
                .Include(s => s.Teacher).ThenInclude(t => t.User)
                .Include(b => b.BookingStatus)
                .Include(b => b.Payments)
                .Where(b => b.BookingStatusId == statusId && !b.IsDeleted)
                .ToListAsync();
        }

        /// <summary>
        /// Kiểm tra xem khoảng thời gian có conflict với booking đã thanh toán không
        /// </summary>
        public async Task<bool> CheckBookingConflictWithPaidAsync(int teacherId, DateTime start, DateTime end)
        {
            return await _context.Bookings
                .Where(b => b.TeacherId == teacherId
                            && !b.IsDeleted
                            && b.IsPaid == true
                            && b.RequestedStartTime < end
                            && b.RequestedEndTime > start)
                .AnyAsync();
        }

        public async Task<bool> CheckBookingConflictWithConfirmedAsync(int teacherId, DateTime start, DateTime end, int? excludeBookingId = null)
        {
            var query = _context.Bookings
                .Where(b => b.TeacherId == teacherId
                            && !b.IsDeleted
                            && b.BookingStatusId == 2 // Confirmed
                            && b.RequestedStartTime < end
                            && b.RequestedEndTime > start);

            if (excludeBookingId.HasValue)
                query = query.Where(b => b.BookingId != excludeBookingId.Value);

            return await query.AnyAsync();
        }

    }
}
