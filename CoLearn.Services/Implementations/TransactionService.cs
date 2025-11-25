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
            var responses = _mapper.Map<List<TransactionResponse>>(transactions);

            // cache để tránh gọi nhiều lần cùng 1 user
            var userCache = new Dictionary<int, User>();

            for (int i = 0; i < transactions.Count; i++)
            {
                var tx = transactions[i];
                var resp = responses[i];

                // Lấy payment để biết payerUserId
                var payment = await _unitOfWork.PaymentRepository.GetByIdAsync((int)tx.PaymentId);
                if (payment == null)
                    continue;

                var payerUserId = payment.PayerUserId;

                if (!userCache.TryGetValue(payerUserId ?? 0, out var user))
                {
                    user = await _unitOfWork.UserRepository.GetByIdAsync(payerUserId ?? 0);
                    userCache[payerUserId ?? 0] = user; // user có thể là null
                }

                if (user != null)
                {
                    resp.FullName = user.FullName;
                    resp.Email = user.Email;
                    resp.Phone = user.Phone;
                }
            }

            return responses;
        }

        public async Task<List<TransactionResponse>> GetByPaymentIdAsync(long paymentId)
        {
            var transactions = await _unitOfWork.TransactionRepository.GetByPaymentIdAsync(paymentId);
            var responses = _mapper.Map<List<TransactionResponse>>(transactions);

            if (!transactions.Any())
                return responses;

            // Lấy payment 1 lần
            var payment = await _unitOfWork.PaymentRepository.GetByIdAsync((int)paymentId);
            User user = null;
            if (payment != null)
                user = await _unitOfWork.UserRepository.GetByIdAsync(payment.PayerUserId ?? 0);

            foreach (var resp in responses)
            {
                if (user != null)
                {
                    resp.FullName = user.FullName;
                    resp.Email = user.Email;
                    resp.Phone = user.Phone;
                }
            }

            return responses;
        }

        public async Task<List<TransactionResponse>> GetByUserIdAsync(int userId)
        {
            // Ở repo bạn có method lọc theo userId — repo đã lọc Payment.PayerUserId == userId
            var transactions = await _unitOfWork.TransactionRepository.GetByUserIdAsync(userId);
            var responses = _mapper.Map<List<TransactionResponse>>(transactions);

            // Lấy user 1 lần
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            foreach (var resp in responses)
            {
                if (user != null)
                {
                    resp.FullName = user.FullName;
                    resp.Email = user.Email;
                    resp.Phone = user.Phone;
                }
            }

            return responses;
        }

        public async Task<TransactionResponse?> GetByGatewayCodeAsync(string gatewayCode)
        {
            var transaction = await _unitOfWork.TransactionRepository.GetByGatewayCodeAsync(gatewayCode);
            if (transaction == null)
                return null;

            var resp = _mapper.Map<TransactionResponse>(transaction);
            var user = await _unitOfWork.UserRepository.GetByIdAsync(resp.PaymentDetail.PayerUserId ?? 0);
            if (user != null)
            {
                resp.FullName = user.FullName;
                resp.Email = user.Email;
                resp.Phone = user.Phone;
            }

            return resp;
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
