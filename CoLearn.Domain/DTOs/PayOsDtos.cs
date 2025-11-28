using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CoLearn.Domain.DTOs
{
    // ============================================================
    // ✅ 1. CREATE PAYMENT LINK - RESPONSE
    // ============================================================
    public class PayOSCreateResponse
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("desc")]
        public string Desc { get; set; }

        [JsonPropertyName("data")]
        public PayOSCreateResponseData Data { get; set; }

        // Helper: lấy link checkout nhanh
        public string CheckoutUrl => Data?.CheckoutUrl ?? string.Empty;
    }

    public class PayOSCreateResponseData
    {
        [JsonPropertyName("orderCode")]
        public long OrderCode { get; set; }

        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("checkoutUrl")]
        public string CheckoutUrl { get; set; }

        [JsonPropertyName("qrCode")]
        public string QrCode { get; set; }

        // PayOS trả expiredAt = UNIX timestamp (số giây kể từ 1970)
        [JsonPropertyName("expiredAt")]
        public long? ExpiredAt { get; set; }

        // Danh sách sản phẩm / dịch vụ (nếu có)
        [JsonPropertyName("items")]
        public List<PayOSItem> Items { get; set; } = new();
    }

    // ============================================================
    // ✅ 2. CREATE PAYMENT LINK - REQUEST (optional, nếu bạn cần gửi)
    // ============================================================
    public class PayOSCreateRequest
    {
        [JsonPropertyName("orderCode")]
        public long OrderCode { get; set; }

        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("returnUrl")]
        public string ReturnUrl { get; set; }

        [JsonPropertyName("buyerName")]
        public string BuyerName { get; set; }

        [JsonPropertyName("buyerEmail")]
        public string BuyerEmail { get; set; }

        [JsonPropertyName("buyerPhone")]
        public string BuyerPhone { get; set; }


        [JsonPropertyName("cancelUrl")]
        public string CancelUrl { get; set; }

        [JsonPropertyName("items")]
        public List<PayOSItem> Items { get; set; } = new();
    }

    // ============================================================
    // ✅ 3. PAYOS ITEM - dùng chung cho cả request & response
    // ============================================================
    public class PayOSItem
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("price")]
        public int Price { get; set; }
    }

    // ============================================================
    // ✅ 4. WEBHOOK PAYLOAD (PayOS gửi về khi có thanh toán)
    // ============================================================
    public class PayOSWebhookPayload
    {
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("desc")]
        public string? Desc { get; set; }

        [JsonPropertyName("data")]
        public PayOSWebhookData? Data { get; set; } // Sửa thành nullable để an toàn

        [JsonPropertyName("signature")]
        public string? Signature { get; set; } 
    }

    // Class con, chỉ chứa những gì có trong object "data"

    public class PayOSWebhookData
    {
        [JsonPropertyName("orderCode")]
        public long OrderCode { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("accountNumber")]
        public string? AccountNumber { get; set; } // THÊM

        [JsonPropertyName("reference")]
        public string? Reference { get; set; }

        [JsonPropertyName("transactionDateTime")]
        public string? TransactionDateTime { get; set; }

        [JsonPropertyName("paymentLinkId")]
        public string? PaymentLinkId { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("desc")]
        public string? Desc { get; set; }

        [JsonPropertyName("currency")]
        public string? Currency { get; set; } // THÊM

        [JsonPropertyName("counterAccountBankId")]
        public string? CounterAccountBankId { get; set; } // THÊM

        [JsonPropertyName("counterAccountBankName")]
        public string? CounterAccountBankName { get; set; } // THÊM

        [JsonPropertyName("counterAccountName")]
        public string? CounterAccountName { get; set; } // THÊM

        [JsonPropertyName("counterAccountNumber")]
        public string? CounterAccountNumber { get; set; } // THÊM

        [JsonPropertyName("virtualAccountName")]
        public string? VirtualAccountName { get; set; } // THÊM

        [JsonPropertyName("virtualAccountNumber")]
        public string? VirtualAccountNumber { get; set; } // THÊM

        [JsonPropertyName("items")]
        public List<PayOSItem> Items { get; set; } = new();
    }
}
