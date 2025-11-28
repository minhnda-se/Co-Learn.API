using AutoMapper;
using CoLearn.Domain.DTOs;
using CoLearn.Domain.Interfaces.Services;
using CoLearn.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Services.Handler
{
    public class BookingJobHandler
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        public BookingJobHandler(IUnitOfWork unitOfWork, INotificationService notificationService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _mapper = mapper;
        }

        public async Task SendPaymentReminderAsync(int bookingId)
        {
            var booking = await _unitOfWork.BookingRepository.GetByIdAsync(bookingId);
            if (booking == null || booking.IsPaid) return;

            var info = _mapper.Map<BookingEmailDto>(booking);
            await _notificationService.SendPaymentReminderAsync(info);
        }

        public async Task AutoCancelUnpaidBookingAsync(int bookingId)
        {
            var booking = await _unitOfWork.BookingRepository.GetByIdAsync(bookingId);
            if (booking == null || booking.IsPaid) return;

            booking.BookingStatusId = 3; // Cancelled
            booking.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.BookingRepository.UpdateAndSaveAsync(booking);
            await _unitOfWork.CommitAsync();

            var info = _mapper.Map<BookingEmailDto>(booking);
            await _notificationService.SendBookingCancelledAsync(info);
        }
    }
}