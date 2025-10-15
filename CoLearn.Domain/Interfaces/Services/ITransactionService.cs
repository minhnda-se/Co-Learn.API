
using CoLearn.Domain.Models;

namespace CoLearn.Domain.Interfaces.Services
{
    public interface ITransactionService
    {
        Task<List<Transaction>> GetAllTransactionsAsync();
        Task<List<Transaction>> GetByPaymentIdAsync(long paymentId);
        Task<Transaction?> GetByGatewayCodeAsync(string gatewayCode);
        Task<Transaction> CreateTransactionAsync(Transaction transaction);
    }
}
