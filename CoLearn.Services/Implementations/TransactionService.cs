using AutoMapper;
using CoLearn.Domain.DTOs;
using CoLearn.Domain.DTOs.Responses;
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
        private readonly IMapper _mapper;

        public TransactionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<TransactionResponse>> GetAllTransactionsAsync()
        {
            var transactions = await _unitOfWork.TransactionRepository.GetAllAsync();
            return _mapper.Map<List<TransactionResponse>>(transactions);

        }

        public async Task<List<TransactionResponse>> GetByPaymentIdAsync(long paymentId)
        {
            var transactions = await _unitOfWork.TransactionRepository.GetByPaymentIdAsync(paymentId);
            return _mapper.Map<List<TransactionResponse>>(transactions);
        }

        public async Task<List<TransactionResponse>> GetByUserIdAsync(int userId)
        {
            var transactions = await _unitOfWork.TransactionRepository.GetByUserIdAsync(userId);
            return _mapper.Map<List<TransactionResponse>>(transactions);
        }

        public async Task<TransactionResponse?> GetByGatewayCodeAsync(string gatewayCode)
        {
            var transaction =  await _unitOfWork.TransactionRepository.GetByGatewayCodeAsync(gatewayCode);
            return _mapper?.Map<TransactionResponse>(transaction);
        }

        public async Task<long> CreateTransactionAsync(TransactionRequest transaction)
        {
            if (transaction == null)
            {
                return 0;
            }
            var entity = _mapper.Map<Transaction>(transaction);
            await _unitOfWork.TransactionRepository.AddAsync(entity);
            await _unitOfWork.TransactionRepository.SaveChangesAsync();
            return entity.TransactionId;
        }
    }
}
