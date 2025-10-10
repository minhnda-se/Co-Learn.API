using CoLearn.Domain.DTOs;
using CoLearn.Domain.Interfaces.Repositories;
using CoLearn.Domain.Models;
using CoLearn.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Infrastructure.Repositories
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        private readonly AppDbContext _context;
        public PaymentRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Payment> CreateAsync(CreateRequest request)
        {
            var payment = new Payment
            {
                BookingId = request.BookingId,
                EnrollmentId = request.EnrollmentId,
                PayerUserId = request.PayerUserId,
                Amount = request.Amount,
                MethodId = request.MethodId,
                StatusId = 1, // pending
                CreatedAt = DateTime.UtcNow
            };
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return payment;
        }

        public async Task<Payment?> GetByIdAsync(long paymentId)
        {
            return await _context.Payments
                .Include(p => p.Method)
                .Include(p => p.Status)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId && !p.IsDeleted);
        }

        public async Task<bool> SaveTransactionAsync(Return vnPayReturn)
        {
            var transaction = new Transaction
            {
                PaymentId = long.Parse(vnPayReturn.vnp_TxnRef),
                CreatedAt = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task UpdateStatusAsync(long paymentId, int statusId, string? transactionNo)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.PaymentId == paymentId);
            if (payment != null)
            {
                payment.StatusId = statusId;
                payment.UpdatedAt = DateTime.UtcNow;

                if (!string.IsNullOrEmpty(transactionNo))
                {
                    var transaction = new Transaction
                    {
                        PaymentId = paymentId,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.Transactions.Add(transaction);
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            return await _context.Payments
                .Include(p => p.Method)
                .Include(p => p.Status)
                .Where(p => !p.IsDeleted)
                .ToListAsync();
        }

        


    }
}
