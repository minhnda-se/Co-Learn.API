using CoLearn.Domain.DTOs;
using CoLearn.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Domain.Interfaces.Repositories
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        Task<Payment> CreateAsync(CreateRequest request);
        Task<int> CreatePaymentAsync(Payment payment);

        Task<Payment?> GetByIdAsync(long paymentId);
        Task<Payment> GetByBookingIdAsync(int bookingId);
        Task<Payment> GetByEnrollmentIdAsync(int enrollmentId);
        Task UpdateStatusAsync(long paymentId, int statusId, string? transactionNo);

        Task<bool> SaveTransactionAsync(Return vnPayReturn);

        Task<IEnumerable<Payment>> GetAllAsync();
    }
}
