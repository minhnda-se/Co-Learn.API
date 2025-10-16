using CoLearn.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CoLearn.Infrastructure.Extensions
{
    public class Vnpay
    {
        private readonly string _tmnCode;
        private readonly string _hashSecret;
        private readonly string _baseUrl;
        private readonly string _returnUrl;

        public Vnpay(string tmnCode, string hashSecret, string baseUrl, string returnUrl)
        {
            _tmnCode = tmnCode;
            _hashSecret = hashSecret;
            _baseUrl = baseUrl;
            _returnUrl = returnUrl;
        }

        public string GetPaymentUrl(VnpayPayRequest request)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["vnp_Amount"] = ((int)(request.Amount * 100)).ToString();
            query["vnp_TxnRef"] = request.OrderId;
            query["vnp_OrderInfo"] = request.OrderInfo;
            query["vnp_CreateDate"] = request.CreatedDate.ToString("yyyyMMddHHmmss");
            query["vnp_ReturnUrl"] = request.ReturnUrl;
            query["vnp_IpAddr"] = request.IpAddress;
            query["vnp_Locale"] = request.Locale;

            // (Bước tính checksum)
            var rawData = string.Join("&", query.AllKeys.Select(k => $"{k}={query[k]}"));
            var secureHash = HmacSHA512(_hashSecret, rawData);
            query["vnp_SecureHash"] = secureHash;

            return $"{_baseUrl}?{query}";
        }

        private string HmacSHA512(string key, string data)
        {
            using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key)))
            {
                var hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hashValue).Replace("-", "").ToLower();
            }
        }
    }

}
