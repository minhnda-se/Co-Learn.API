using CoLearn.Domain.Common;
using CoLearn.Domain.DTOs;
using CoLearn.Domain.Interfaces;
using CoLearn.Domain.Interfaces.Repositories;
using CoLearn.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VNPAY.NET;
using VNPAY.NET.Enums;
using VNPAY.NET.Models;

namespace CoLearn.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly Vnpay _vnpay;

        public PaymentService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _vnpay = new Vnpay();
            _vnpay.Initialize(
                _configuration["Vnpay:TmnCode"],
                _configuration["Vnpay:HashSecret"],
                _configuration["Vnpay:BaseUrl"],
                _configuration["Vnpay:ReturnUrl"]
            );
        }

        public async Task<Result<CreateResponse>> CreateAsync(CreateRequest dto)
        {
            var payment = await _unitOfWork.PaymentRepository.CreateAsync(dto);
            var vnpRequest = new PaymentRequest
            {
                PaymentId = payment.PaymentId,
                Description = $"Thanh toán cho đơn #{payment.PaymentId}",
                Money = (double)payment.Amount,
                IpAddress = "127.0.0.1",
                BankCode = BankCode.ANY, // nếu không chọn ngân hàng cụ thể
                CreatedDate = DateTime.Now,
                Currency = Currency.VND,
                Language = DisplayLanguage.Vietnamese
            };

            var paymentUrl = _vnpay.GetPaymentUrl(vnpRequest);

            var response = new CreateResponse
            {
                PaymentId = payment.PaymentId,
                PaymentUrl = paymentUrl
            };

            return Result<CreateResponse>.Success(response);
        }

        public async Task<Result<IEnumerable<Detail>>> GetAllAsync()
        {
            var payments = await _unitOfWork.PaymentRepository.GetAllAsync();

            var result = payments.Select(p => new Detail
            {
                PaymentId = p.PaymentId,
                BookingId = p.BookingId,
                EnrollmentId = p.EnrollmentId,
                PayerUserId = p.PayerUserId,
                Amount = p.Amount,
                MethodId = p.MethodId ?? 0,
                StatusId = p.StatusId,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                MethodName = p.Method?.MethodName ?? "",
                StatusName = p.Status?.StatusName ?? ""
            });

            return Result<IEnumerable<Detail>>.Success(result);
        }

        public async Task<Result<Detail?>> GetByIdAsync(long id)
        {
            var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(id);
            if (payment == null)
                return Result<Detail?>.Failure("Không tìm thấy Payment");

            var dto = new Detail
            {
                PaymentId = payment.PaymentId,
                BookingId = payment.BookingId,
                EnrollmentId = payment.EnrollmentId,
                PayerUserId = payment.PayerUserId,
                Amount = payment.Amount,
                MethodId = payment.MethodId ?? 0,
                StatusId = payment.StatusId,
                CreatedAt = payment.CreatedAt,
                UpdatedAt = payment.UpdatedAt,
                MethodName = payment.Method?.MethodName ?? "",
                StatusName = payment.Status?.StatusName ?? ""
            };

            return Result<Detail?>.Success(dto);
        }

        public async Task<Result<bool>> HandleVnPayReturnAsync(IQueryCollection query)
        {
            var result = _vnpay.GetPaymentResult(query);

            if (!result.IsSuccess)
                return Result<bool>.Failure($"Thanh toán thất bại: {result.PaymentResponse.Description}");

            // Cập nhật database
            await _unitOfWork.PaymentRepository.UpdateStatusAsync(
                result.PaymentId,
                2, // 2 = success
                result.VnpayTransactionId.ToString()
            );

            return Result<bool>.Success(true);

        }
    }
}
