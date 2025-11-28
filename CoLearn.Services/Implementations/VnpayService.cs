using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Services.Implementations
{
    public class VnpayService
    {
        public bool ValidateVnPaySignature(Dictionary<string, string> vnpParams, string hashSecret)
        {
            var vnpSecureHash = vnpParams["vnp_SecureHash"];
            vnpParams.Remove("vnp_SecureHash");
            vnpParams.Remove("vnp_SecureHashType");

            var sorted = vnpParams.OrderBy(x => x.Key);
            var signData = string.Join("&", sorted.Select(x => $"{x.Key}={x.Value}"));

            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(hashSecret));
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(signData));
            var computedHash = BitConverter.ToString(hashBytes).Replace("-", "").ToUpper();

            return computedHash == vnpSecureHash;
        }
    }
}
