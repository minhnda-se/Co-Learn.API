using CoLearn.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Domain.Interfaces
{
    public interface IPayOSService
    {
        Task<string> CreatePaymentUrlAsync(int userId, int orderId, decimal amount, string description, string itemName, int type);

        Task<string> CreateBookingPaymentAsync(int bookingId, int userId);

        Task<string> CreateCoursePaymentAsync(int courseId, int studentId, int userId);

        Task HandleWebhookAsync(PayOSWebhookPayload payload);

        bool VerifySignature(PayOSWebhookPayload payload);
    }
}
