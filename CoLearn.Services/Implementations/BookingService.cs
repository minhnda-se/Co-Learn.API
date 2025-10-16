using AutoMapper;
using CoLearn.Domain.Common;
using CoLearn.Domain.DTOs;
using CoLearn.Domain.Enums;
using CoLearn.Domain.Interfaces;
using CoLearn.Domain.Interfaces.Services;
using CoLearn.Domain.Models;
using CoLearn.Services.Exceptions;
using CoLearn.Services.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CoLearn.Domain.DTOs.BookingDtos;

namespace CoLearn.Services.Implementations
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;
        private readonly IBackgroundJobService _backgroundJobService;

        public BookingService(IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService, IBackgroundJobService backgroundJobService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _notificationService = notificationService;
            _backgroundJobService = backgroundJobService;
        }

        public async Task<Result<BookingResponseDto?>> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.BookingRepository.GetByIdAsync(id);
            if (entity == null) return Result<BookingResponseDto?>.Failure("Booking not found");
            return Result<BookingResponseDto?>.Success(_mapper.Map<BookingResponseDto>(entity));
        }

        public async Task<Result<PagedResult<BookingResponseDto>>> GetAllAsync(int pageIndex, int pageSize)
        {
            var query = (await _unitOfWork.BookingRepository.GetAllBookingsAsync()).AsQueryable();
            return BuildPagedResult(query, pageIndex, pageSize);
        }

        public async Task<Result<PagedResult<BookingResponseDto>>> GetByStudentIdAsync(int studentId, int pageIndex, int pageSize)
        {
            var query = (await _unitOfWork.BookingRepository.GetByStudentIdAsync(studentId)).AsQueryable();
            return BuildPagedResult(query, pageIndex, pageSize);
        }
        public async Task<Result<PagedResult<BookingResponseDto>>> GetByParentIdAsync(int parentId, int pageIndex, int pageSize)
        {
            var query = (await _unitOfWork.BookingRepository.GetByParentIdAsync(parentId)).AsQueryable();
            return BuildPagedResult(query, pageIndex, pageSize);
        }

        public async Task<Result<PagedResult<BookingResponseDto>>> GetByTeacherIdAsync(int teacherId, int pageIndex, int pageSize)
        {
            var query = (await _unitOfWork.BookingRepository.GetByTeacherIdAsync(teacherId)).AsQueryable();
            return BuildPagedResult(query, pageIndex, pageSize);
        }

        public async Task<Result<PagedResult<BookingResponseDto>>> GetByStatusIdAsync(int statusId, int pageIndex, int pageSize)
        {
            var query = (await _unitOfWork.BookingRepository.GetByStatusIdAsync(statusId)).AsQueryable();
            return BuildPagedResult(query, pageIndex, pageSize);
        }

        public async Task<Result<string>> ConfirmBookingAsync(int bookingId)
        {
            var booking = await _unitOfWork.BookingRepository.GetByIdAsync(bookingId);
            if (booking == null || booking.IsDeleted)
                return Result<string>.Failure("Booking không tồn tại.");

            if (booking.BookingStatusId != 1) // 1 = Pending
                return Result<string>.Failure("Booking không ở trạng thái chờ xác nhận.");

            // Check conflict với các booking đã được confirm
            bool hasConflict = await _unitOfWork.BookingRepository.CheckBookingConflictWithConfirmedAsync(
                booking.TeacherId,
                booking.RequestedStartTime ?? DateTime.MinValue,
                booking.RequestedEndTime ?? DateTime.MinValue,
                excludeBookingId: booking.BookingId
            );

            if (hasConflict)
                return Result<string>.Failure("Lịch này bị trùng với một buổi học khác đã được xác nhận.");

            booking.BookingStatusId = 2; // Confirmed
            booking.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.BookingRepository.UpdateAndSaveAsync(booking);
            await _unitOfWork.CommitAsync();

            // Gửi email cho parent
            try
            {
                var info = _mapper.Map<BookingEmailDto>(booking);
                await _notificationService.SendBookingConfirmedAsync(info);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email error: {ex.Message}");
            }

            await _backgroundJobService.DeleteByTargetAsync("Booking", bookingId);
            // ✅ Schedule reminder sau 7 phút
            _backgroundJobService.Schedule<BookingJobHandler>(
                x => x.SendPaymentReminderAsync(bookingId),
                TimeSpan.FromMinutes(7),
                JobType.BookingReminder,
                "Booking",
                bookingId
);


            // ✅ Schedule cancel sau 10 phút
            _backgroundJobService.Schedule<BookingJobHandler>(
                x => x.AutoCancelUnpaidBookingAsync(bookingId),
                TimeSpan.FromMinutes(10),
                JobType.AutoCancel,
                "Booking",
                bookingId
);

            return Result<string>.Success("Booking đã được xác nhận thành công.");
        }
        public async Task<Result<string>> DeclineBookingAsync(int bookingId)
        {
            var booking = await _unitOfWork.BookingRepository.GetByIdAsync(bookingId);
            if (booking == null || booking.IsDeleted)
                return Result<string>.Failure("Booking không tồn tại.");

            if (booking.BookingStatusId != 1) // 1 = Pending
                return Result<string>.Failure("Booking không ở trạng thái chờ xác nhận.");

            booking.BookingStatusId = 3; // Declined
            booking.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.BookingRepository.UpdateAndSaveAsync(booking);
            await _unitOfWork.CommitAsync();

            // 🔹 Gửi email thông báo
            try
            {
                var info = _mapper.Map<BookingEmailDto>(booking);
                await _notificationService.SendBookingDeclineAsync(info);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email error: {ex.Message}");
            }

            // 🔹 Xóa toàn bộ job Hangfire liên quan đến booking này
            try
            {
                var deletedCount = await _backgroundJobService.DeleteByTargetAsync("Booking", bookingId);
                Console.WriteLine($"Deleted {deletedCount} related Hangfire jobs for BookingID={bookingId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to delete related jobs: {ex.Message}");
            }

            return Result<string>.Success("Booking đã bị từ chối.");
        }



        public async Task<int> CreateAsync(BookingRequestDto dto)
        {
            var entity = _mapper.Map<Booking>(dto);
            entity.CreatedAt = DateTime.UtcNow;
            entity.IsDeleted = false;
            entity.BookingStatusId = 1; // Pending
            entity.IsPaid = false;      // mặc định chưa thanh toán

            // ✅ Check conflict
            bool hasConflict = await _unitOfWork.BookingRepository
                .CheckBookingConflictWithPaidAsync(entity.TeacherId,
                                           entity.RequestedStartTime.Value,
                                           entity.RequestedEndTime.Value);

            if (hasConflict)
            {
                var date = DateOnly.FromDateTime(entity.RequestedStartTime.Value);
                var occupiedSlots = await _unitOfWork.BookingRepository
                    .GetOccupiedSlotsAsync(entity.TeacherId, date);
                throw new BookingConflictException(
                    $"Thời gian này đã có booking được thanh toán.",
                    400
                )
                {
                    Data = { ["occupiedSlots"] = occupiedSlots }
                };
            }

            // ✅ Save booking
            await _unitOfWork.BookingRepository.AddAndSaveAsync(entity);
            await _unitOfWork.CommitAsync();

            // Load booking full info
            var full = await _unitOfWork.BookingRepository.GetByIdAsync(entity.BookingId);
            if (full != null)
            {
                var bookingInfo = _mapper.Map<BookingEmailDto>(full);
                try
                {
                    await _notificationService.SendBookingCreatedAsync(bookingInfo);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send booking created email: {ex.Message}");
                }
            }

            return entity.BookingId;
        }


        public async Task<int> UpdateAsync(int id, BookingRequestDto dto)
        {
            var entity = await _unitOfWork.BookingRepository.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted) return -1;

            _mapper.Map(dto, entity);
            entity.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.BookingRepository.UpdateAndSaveAsync(entity);
            return entity.BookingId;
        }
        public async Task<int> UpdateStatusAsync(int id, int statusId)
        {
            var entity = await _unitOfWork.BookingRepository.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted) return -1;

            entity.BookingStatusId = statusId;
            entity.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.BookingRepository.UpdateAndSaveAsync(entity);
            return entity.BookingId;
        }

        public async Task<int> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.BookingRepository.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted) return -1;

            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;

            await _unitOfWork.BookingRepository.UpdateAndSaveAsync(entity);
            return entity.BookingId;
        }

        private Result<PagedResult<BookingResponseDto>> BuildPagedResult(IQueryable<Booking> query, int pageIndex, int pageSize)
        {
            var totalCount = query.Count();
            var items = query
                .OrderByDescending(b => b.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var dtoItems = _mapper.Map<List<BookingResponseDto>>(items);

            return Result<PagedResult<BookingResponseDto>>.Success(
                new PagedResult<BookingResponseDto>(dtoItems, pageIndex, pageSize, totalCount)
            );
        }
    }
}
