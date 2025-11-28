using CoLearn.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoLearn.Domain.Interfaces.Repositories
{
    public interface ITransactionRepository : IGenericRepository<Transaction>
    {
        Task<List<Transaction>> GetAllAsync();
        Task<List<Transaction>> GetByPaymentIdAsync(long paymentId);
        Task<List<Transaction>> GetByUserIdAsync(int userId);

        Task<Transaction?> GetByGatewayCodeAsync(string gatewayCode);
        Task AddAsync(Transaction transaction);
        Task SaveChangesAsync();
    }
}
