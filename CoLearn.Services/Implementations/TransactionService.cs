using CoLearn.Domain.Interfaces;
using CoLearn.Domain.Interfaces.Repositories;
using CoLearn.Domain.Interfaces.Services;
using CoLearn.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Services.Implementations
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransactionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Transaction>> GetAllTransactionsAsync()
        {
            return await _unitOfWork.TransactionRepository.GetAllAsync();
        }

        public async Task<List<Transaction>> GetByPaymentIdAsync(long paymentId)
        {
            return await _unitOfWork.TransactionRepository.GetByPaymentIdAsync(paymentId);
        }

        public async Task<Transaction?> GetByGatewayCodeAsync(string gatewayCode)
        {
            return await _unitOfWork.TransactionRepository.GetByGatewayCodeAsync(gatewayCode);
        }

        public async Task<Transaction> CreateTransactionAsync(Transaction transaction)
        {
            await _unitOfWork.TransactionRepository.AddAsync(transaction);
            await _unitOfWork.TransactionRepository.SaveChangesAsync();
            return transaction;
        }
    }
}
