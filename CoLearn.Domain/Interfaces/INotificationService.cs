using CoLearn.Domain.DTOs;
using System.Threading.Tasks;

namespace CoLearn.Domain.Interfaces.Services
{
    /// <summary>
    /// Service phụ trách gửi thông báo (email, sms, push...)
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Gửi email khi có booking mới được tạo.
        /// </summary>
        Task SendBookingCreatedAsync(BookingEmailDto bookingDto);

        /// <summary>
        /// Gửi email nhắc nhở trước giờ học (cho teacher và student).
        /// </summary>
        Task SendReminderAsync(BookingEmailDto bookingDto);

        /// <summary>
        /// Gửi email khi teacher xác nhận buổi học.
        /// </summary>
        Task SendBookingConfirmedAsync(BookingEmailDto bookingDto);
        Task SendBookingDeclineAsync(BookingEmailDto bookingDto);
        Task SendPaymentReminderAsync(BookingEmailDto bookingDto);
        Task SendBookingCancelledAsync(BookingEmailDto bookingDto);

        Task SendEmailAsync(string to, string subject, string htmlBody);


    }
}
