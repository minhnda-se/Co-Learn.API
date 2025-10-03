using CoLearn.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoLearn.Domain.Interfaces.Repositories
{
    public interface IBookingRepository : IGenericRepository<Booking>
    {
        Task<Booking?> GetByIdAsync(int id);
        Task<List<Booking>> GetAllBookingsAsync();
        Task<List<Booking>> GetByStudentIdAsync(int studentId);
        Task<List<Booking>> GetByParentIdAsync(int parentId);
        Task<List<Booking>> GetByScheduleIdAsync(int scheduleId);
        Task<List<Booking>> GetByTeacherIdAsync(int teacherId);
        Task<List<Booking>> GetByStatusIdAsync(int statusId);
    }
}
