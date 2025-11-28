using CoLearn.Domain.Interfaces.Repositories;
using CoLearn.Domain.Models;
using CoLearn.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoLearn.Infrastructure.Repositories
{
    public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
    {

        public TransactionRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<Transaction>> GetAllAsync()
        {
            return await _context.Transactions
                .Include(t => t.Payment).ThenInclude(p => p.Status)
                .Include(t => t.Payment).ThenInclude(p => p.Method)
                .ToListAsync();
        }

        public async Task<List<Transaction>> GetByPaymentIdAsync(long paymentId)
        {
            return await _context.Transactions
                .Include(t => t.Payment).ThenInclude(p => p.Status)
                .Include(t => t.Payment).ThenInclude(p => p.Method)
                .Where(t => t.PaymentId == paymentId)
                .ToListAsync();
        }

        public async Task<List<Transaction>> GetByUserIdAsync(int userId)
        {
            return await _context.Transactions
                .Include(t => t.Payment).ThenInclude(p => p.Status)
                .Include(t => t.Payment).ThenInclude(p => p.Method)
                .Where(t => t.Payment.PayerUserId == userId)
                .ToListAsync();
        }

        public async Task<Transaction?> GetByGatewayCodeAsync(string gatewayCode)
        {
            return await _context.Transactions
                .Include(t => t.Payment).ThenInclude(p => p.Status)
                .Include(t => t.Payment).ThenInclude(p => p.Method)
                .FirstOrDefaultAsync(t => t.GatewayTransactionCode == gatewayCode);
        }

        public async Task AddAsync(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
