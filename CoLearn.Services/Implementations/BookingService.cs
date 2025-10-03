using AutoMapper;
using CoLearn.Domain.Common;
using CoLearn.Domain.DTOs;
using CoLearn.Domain.Interfaces;
using CoLearn.Domain.Interfaces.Services;
using CoLearn.Domain.Models;
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

        public BookingService(IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _notificationService = notificationService;
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

        public async Task<Result<PagedResult<BookingResponseDto>>> GetByScheduleIdAsync(int scheduleId, int pageIndex, int pageSize)
        {
            var query = (await _unitOfWork.BookingRepository.GetByScheduleIdAsync(scheduleId)).AsQueryable();
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

        public async Task<int> CreateAsync(BookingRequestDto dto)
        {
            var entity = _mapper.Map<Booking>(dto);
            entity.CreatedAt = DateTime.UtcNow;
            entity.IsDeleted = false;
            entity.BookingStatusId = 1; // Pending
            entity.IsPaid = false;      // mặc định chưa thanh toán

            // ✅ Check conflict
            bool hasConflict = await _unitOfWork.BookingRepository
                .CheckBookingConflictAsync(entity.TeacherId,
                                           entity.RequestedStartTime.Value,
                                           entity.RequestedEndTime.Value);

            if (hasConflict)
            {
                throw new InvalidOperationException("Thời gian này đã có booking được thanh toán.");
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
