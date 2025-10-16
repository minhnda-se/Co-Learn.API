using CoLearn.Domain.DTOs;
using CoLearn.Domain.Enums;
using CoLearn.Domain.Interfaces;
using CoLearn.Domain.Interfaces.Services;
using CoLearn.Domain.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Linq; // Cần thêm using này
using System.Collections.Generic; // Cần thêm using này

namespace CoLearn.Infrastructure.Services.Payments
{
    public class PayOSService : IPayOSService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;

        public PayOSService(IConfiguration config, IUnitOfWork unitOfWork)
        {
            _config = config;
            _unitOfWork = unitOfWork;

            var baseUrl = _config["PayOS:BaseUrl"];
            _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
            _httpClient.DefaultRequestHeaders.Add("x-client-id", _config["PayOS:ClientId"]);
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _config["PayOS:ApiKey"]);
        }

        // ============================================================
        // =============== SIGNATURE GENERATION (CORRECTED) =============
        // ============================================================
        private string GenerateSignature(long orderCode, int amount, string description, string returnUrl, string cancelUrl)
        {
            var checksumKey = _config["PayOS:ChecksumKey"];

            // Chuỗi dữ liệu để tạo signature CHỈ bao gồm 5 trường, được sắp xếp theo alphabet
            var rawData = $"amount={amount}&cancelUrl={cancelUrl}&description={description}&orderCode={orderCode}&returnUrl={returnUrl}";

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(checksumKey));
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }


        // ============================================================
        // =============== CREATE PAYMENT REQUEST =====================
        // ============================================================
        public async Task<string> CreatePaymentUrlAsync(int userId, int orderId, decimal amount, string description, string itemName, int type)
        {
            // BƯỚC 1: Tạo một bản ghi Payment trong DB để lấy PaymentID làm orderCode
            var payment = new Payment
            {
                Amount = amount,
                MethodId = (int)PaymentMethodEnum.PayOS,
                StatusId = (int)StatusEnum.Pending,
                PayerUserId = userId,
                BookingId = type == 1 ? orderId : null,
                EnrollmentId = type == 2 ? orderId : null,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };
            await _unitOfWork.PaymentRepository.CreatePaymentAsync(payment);
            await _unitOfWork.CommitAsync();

            long orderCode = payment.PaymentId;
            var returnUrl = $"{_config["PayOS:ReturnUrl"]}?orderCode={orderCode}";
            var cancelUrl = $"{_config["PayOS:CancelUrl"]}?orderCode={orderCode}";

            // BƯỚC 2: Tạo signature CHỈ từ 5 trường theo đúng tài liệu
            var signature = GenerateSignature(orderCode, (int)amount, description, returnUrl, cancelUrl);

            // BƯỚC 3: Tạo payload đầy đủ để gửi đi (có cả items và signature)
            var finalPayload = new
            {
                orderCode,
                amount = (int)amount,
                description,
                cancelUrl,
                returnUrl,
                items = new[]
                {
            new { name = itemName, quantity = 1, price = (int)amount }
        },
                signature // Chữ ký đã được tạo đúng từ 5 trường
            };

            Console.WriteLine("--- PAYOS REQUEST BODY ---");
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(finalPayload));
            Console.WriteLine("---------------------------");

            var response = await _httpClient.PostAsJsonAsync("v2/payment-requests", finalPayload);
            var responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine("--- PAYOS API RESPONSE ---");
            Console.WriteLine($"Status Code: {response.StatusCode}");
            Console.WriteLine($"Body: {responseBody}");
            Console.WriteLine("--------------------------");

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"PayOS returned {response.StatusCode}: {responseBody}");

            var data = System.Text.Json.JsonSerializer.Deserialize<PayOSCreateResponse>(responseBody, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return data?.Data?.CheckoutUrl ?? "";
        }

        public async Task<string> CreateBookingPaymentAsync(int bookingId, int userId)
        {
            var booking = await _unitOfWork.BookingRepository.GetByIdAsync(bookingId);
            if (booking == null)
                throw new Exception("Booking không tồn tại");
            string description = $"Thanh toán lịch học #{bookingId}";
            string itemName = $"Lịch học #{bookingId}";

            // Chỉ cần gọi CreatePaymentUrlAsync để tạo Payment record và lấy link
            return await CreatePaymentUrlAsync(userId, bookingId, booking.TotalAmount ?? 0, description, itemName, 1);
        }

        public async Task<string> CreateCoursePaymentAsync(int courseId, int studentId, int userId)
        {
            var course = await _unitOfWork.CourseRepository.GetByIdAsync(courseId);
            if (course == null) throw new Exception("Course không tồn tại");

            var enrollment = await _unitOfWork.EnrollmentRepository.FindAsync(e => e.CourseId == courseId && e.StudentId == studentId);
            if (enrollment != null) throw new Exception("Course này đã được thanh toán!");

            enrollment = new Enrollment
            {
                CourseId = courseId,
                StudentId = studentId,
                Status = StatusEnum.OnHold.ToString(),
                UpdatedAt = DateTime.UtcNow
            };
            await _unitOfWork.EnrollmentRepository.AddAndSaveAsync(enrollment);

            string description = $"Thanh toán khóa học #{courseId}";
            string itemName = course.Title; // Thêm itemName

            // SỬA LẠI DÒNG NÀY: Cần cung cấp đủ 6 tham số
            return await CreatePaymentUrlAsync(userId, enrollment.EnrollmentId, course.PricePerSession ?? 0, description, itemName, 2);
        }


        // ============================================================
        // =============== VERIFY SIGNATURE (CHUẨN PAYOS) ==============
        // ============================================================
        public bool VerifySignature(PayOSWebhookPayload payload)
        {
            var key = _config["PayOS:ChecksumKey"];
            var data = payload.Data;

            // Chuyển object data thành Dictionary
            var dict = new SortedDictionary<string, object>
    {
        { "accountNumber", data.AccountNumber ?? "" },
        { "amount", data.Amount },
        { "code", data.Code ?? "" },
        { "counterAccountBankId", data.CounterAccountBankId ?? "" },
        { "counterAccountBankName", data.CounterAccountBankName ?? "" },
        { "counterAccountName", data.CounterAccountName ?? "" },
        { "counterAccountNumber", data.CounterAccountNumber ?? "" },
        { "currency", data.Currency ?? "" },
        { "description", data.Description ?? "" },
        { "orderCode", data.OrderCode },
        { "paymentLinkId", data.PaymentLinkId ?? "" },
        { "reference", data.Reference ?? "" },
        { "transactionDateTime", data.TransactionDateTime ?? "" },
        { "virtualAccountName", data.VirtualAccountName ?? "" },
        { "virtualAccountNumber", data.VirtualAccountNumber ?? "" },
        { "desc", data.Desc ?? "" }
    };

            // Build chuỗi key=value theo alphabet & bỏ null
            var rawData = string.Join("&", dict.Select(kv => $"{kv.Key}={kv.Value}"));

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            var calculatedSignature = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            return calculatedSignature == payload.Signature.ToLower();
        }


        // ============================================================
        // =============== HANDLE WEBHOOK (REFACTORED) ================
        // ============================================================
        public async Task HandleWebhookAsync(PayOSWebhookPayload payload)
        {
            //LUÔN LUÔN KIỂM TRA SIGNATURE
            if (!VerifySignature(payload))
            {
                Console.WriteLine("❌ Invalid webhook signature.");
                throw new Exception("Invalid signature");
            }

            var data = payload.Data;
            if (data == null)
            {
                Console.WriteLine("⚠️ Webhook payload data is null.");
                return; // Dừng lại nếu không có data
            }

            long orderCode = data.OrderCode; // Đây chính là PaymentId của bạn
            var payosStatus = data.Code?.ToUpperInvariant(); // Dùng "code" từ data

            // BƯỚC 1: Dùng orderCode để tìm lại bản ghi Payment
            var payment = await _unitOfWork.PaymentRepository.GetByIdAsync((int)orderCode);
            if (payment == null)
            {
                Console.WriteLine($"⚠️ Payment with OrderCode (PaymentId) {orderCode} not found.");
                return;
            }

            // Nếu giao dịch đã xử lý rồi thì bỏ qua
            if (payment.StatusId != (int)StatusEnum.Pending)
            {
                Console.WriteLine($"ℹ️ Payment {orderCode} has already been processed.");
                return;
            }

            // BƯỚC 2: Cập nhật trạng thái Payment
            bool isSuccess = (payosStatus == "00");
            payment.StatusId = isSuccess ? (int)StatusEnum.Success : (int)StatusEnum.Failed;
            payment.UpdatedAt = DateTime.UtcNow;


            // BƯỚC 3: Cập nhật trạng thái cho Booking hoặc Enrollment
            if (payment.BookingId.HasValue)
            {
                // SỬA LẠI: Logic xử lý cho Booking
                if (isSuccess)
                {
                    // THANH TOÁN THÀNH CÔNG -> TẠO LỊCH HỌC (SCHEDULE)
                    var booking = await _unitOfWork.BookingRepository.GetByIdAsync(payment.BookingId.Value);
                    if (booking != null)
                    {
                        var newSchedule = new Schedule
                        {
                            BookingId = booking.BookingId,
                            TeacherId = booking.TeacherId,
                            StudentId = booking.StudentId,
                            StartTime = booking.RequestedStartTime ?? DateTime.Now,
                            EndTime = booking.RequestedEndTime ?? DateTime.Now,
                            CreatedAt = DateTime.UtcNow,
                            ScheduleStatusId = (int)ScheduleStatusEnum.Scheduled
                        };
                        await _unitOfWork.ScheduleRepository.CreateAsync(newSchedule);
                    }
                }
                // Nếu thanh toán thất bại, chúng ta không cần làm gì cả, vì Schedule chưa được tạo.
            }
            else if (payment.EnrollmentId.HasValue)
            {
                var enrollment = await _unitOfWork.EnrollmentRepository.GetByIdAsync(payment.EnrollmentId.Value);
                // Logic cho Enrollment đã khá đúng: chỉ cập nhật trạng thái
                if (enrollment != null)
                {
                    if (isSuccess)
                    {
                        enrollment.Status = isSuccess ? StatusEnum.Success.ToString() : StatusEnum.Failed.ToString();
                        enrollment.UpdatedAt = DateTime.UtcNow;
                    }
                    else
                    {
                        enrollment.Status = StatusEnum.Failed.ToString();
                        enrollment.UpdatedAt = DateTime.UtcNow;
                        enrollment.DeletedAt = DateTime.UtcNow; // Xoá enrollment nếu thanh toán thất bại
                        enrollment.IsDeleted = true;
                    }
                }
            }

            // =================== THÊM LẠI LOGIC TẠO TRANSACTION ===================
            var transaction = new Transaction
            {
                PaymentId = payment.PaymentId,
                GatewayTransactionCode = data.Reference ?? data.PaymentLinkId ?? "unknown",
                GatewayName = "PayOS",
                GatewayResponse = System.Text.Json.JsonSerializer.Serialize(payload.Data),
                Status = (payment.StatusId == (int)StatusEnum.Success) ? "Success" : "Failed",
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.TransactionRepository.AddAndSaveAsync(transaction);
            // ======================================================================

            await _unitOfWork.CommitAsync();
            Console.WriteLine($"✅ Webhook processed successfully for PaymentId {orderCode}. New status: {payment.StatusId}. Transaction created.");
        }
    }
}