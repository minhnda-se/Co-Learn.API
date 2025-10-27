using CoLearn.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Domain.DTOs
{
    public class TransactionRequest
    {
        public long PaymentId { get; set; }

        public string GatewayTransactionCode { get; set; } = null!;

        public string? GatewayName { get; set; }

        public string? GatewayResponse { get; set; }
    }

    public class TransactionResponse
    {
        public long TransactionId { get; set; }

        public long PaymentId { get; set; }

        public string GatewayTransactionCode { get; set; } = null!;

        public string? GatewayName { get; set; }

        public string? GatewayResponse { get; set; }

        public string Status { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public Detail PaymentDetail { get; set; }
    }
}
