using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Domain.DTOs
{
    public class CreateRequest
    {
        public int? BookingId { get; set; }
        public int? EnrollmentId { get; set; }
        public int PayerUserId { get; set; }
        public decimal Amount { get; set; }
        public int MethodId { get; set; }  // VNPay = 1
        public string? Description { get; set; }
    }
    public class CreateResponse
    {
        public long PaymentId { get; set; }
        public string PaymentUrl { get; set; } = string.Empty;
        public string Message { get; set; } = "Tạo thanh toán thành công";
    }
    public class Return
    {
        public string vnp_TxnRef { get; set; } = string.Empty;
        public string vnp_TransactionNo { get; set; } = string.Empty;
        public string vnp_ResponseCode { get; set; } = string.Empty;
        public string vnp_SecureHash { get; set; } = string.Empty;
        public string vnp_OrderInfo { get; set; } = string.Empty;
        public decimal vnp_Amount { get; set; }
    }
    public class Detail
    {
        public long PaymentId { get; set; }
        public int? BookingId { get; set; }
        public int? EnrollmentId { get; set; }
        public int? PayerUserId { get; set; }
        public decimal Amount { get; set; }
        public int MethodId { get; set; }
        public int StatusId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string MethodName { get; set; } = string.Empty;
    }
    public class StatusUpdate
    {
        public long PaymentId { get; set; }
        public int StatusId { get; set; }
        public string? TransactionNo { get; set; }
    }
}
