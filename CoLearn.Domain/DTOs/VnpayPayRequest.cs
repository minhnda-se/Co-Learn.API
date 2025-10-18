using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Domain.DTOs
{
    public class VnpayPayRequest
    {
        public double Amount { get; set; }
        public DateTime CreatedDate { get; set; }
        public string OrderId { get; set; }
        public string OrderInfo { get; set; }
        public string ReturnUrl { get; set; }
        public string Locale { get; set; }
        public string BankCode { get; set; }
        public string IpAddress { get; set; }
    }
}
