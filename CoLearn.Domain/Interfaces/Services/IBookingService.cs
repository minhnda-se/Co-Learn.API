using CoLearn.Domain.Common;
using CoLearn.Domain.DTOs;
using System.Threading.Tasks;
using static CoLearn.Domain.DTOs.BookingDtos;

namespace CoLearn.Domain.Interfaces.Services
{
    public interface IBookingService
    {
        Task<Result<BookingResponseDto?>> GetByIdAsync(int id);

        Task<Result<PagedResult<BookingResponseDto>>> GetAllAsync(int pageIndex, int pageSize);

        Task<Result<PagedResult<BookingResponseDto>>> GetByStudentIdAsync(int studentId, int pageIndex, int pageSize);
        Task<Result<PagedResult<BookingResponseDto>>> GetByParentIdAsync(int parentId, int pageIndex, int pageSize);
        Task<Result<PagedResult<BookingResponseDto>>> GetByScheduleIdAsync(int scheduleId, int pageIndex, int pageSize);
        Task<Result<PagedResult<BookingResponseDto>>> GetByTeacherIdAsync(int teacher, int pageIndex, int pageSize);
        Task<Result<PagedResult<BookingResponseDto>>> GetByStatusIdAsync(int statusId, int pageIndex, int pageSize);
        Task<Result<string>> ConfirmBookingAsync(int bookingId);

        Task<int> CreateAsync(BookingRequestDto dto);
        Task<int> UpdateAsync(int id, BookingRequestDto dto);
        Task<int> UpdateStatusAsync(int id, int statusId);
        Task<int> DeleteAsync(int id);
    }
}
