
using CoLearn.Domain.DTOs;
using CoLearn.Domain.Models;

namespace CoLearn.Domain.Interfaces.Services
{
    public interface ITransactionService
    {
        Task<List<TransactionResponse>> GetAllTransactionsAsync();
        Task<List<TransactionResponse>> GetByPaymentIdAsync(long paymentId);
        Task<List<TransactionResponse>> GetByUserIdAsync(int userId);

        Task<TransactionResponse?> GetByGatewayCodeAsync(string gatewayCode);
        Task<long> CreateTransactionAsync(TransactionRequest transaction);
    }
}
