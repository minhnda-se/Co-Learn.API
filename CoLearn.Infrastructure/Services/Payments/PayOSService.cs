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
            // 🔹 1. Kiểm tra Payment đang tồn tại và còn trong 15 phút (CreatedAt)
            var existingPayment = await _unitOfWork.PaymentRepository.FindAsync(
                p =>
                    !p.IsDeleted &&
                    p.StatusId != (int)StatusEnum.Success &&
                    p.StatusId != (int)StatusEnum.Failed &&
                    (
                        (type == 1 && p.BookingId == orderId) ||
                        (type == 2 && p.EnrollmentId == orderId)
                    )
            );

            if (existingPayment != null)
            {
                // Nếu chưa quá 15 phút kể từ khi tạo => chặn spam
                if ((DateTime.UtcNow - existingPayment.CreatedAt).TotalMinutes < 15)
                {
                    Console.WriteLine($"⚠️ Existing payment within 15 minutes for order {orderId}, rejecting new request.");
                    return "Bạn đã có một giao dịch đang chờ xử lý. Vui lòng hoàn tất hoặc thử lại sau 15 phút.";
                }
                else
                {
                    // Quá hạn 15 phút → đánh dấu Expired
                    existingPayment.StatusId = (int)StatusEnum.Expired;
                    existingPayment.UpdatedAt = DateTime.UtcNow;
                    await _unitOfWork.PaymentRepository.UpdateAndSaveAsync(existingPayment);
                }
            }

            // 🔹 2. Tạo Payment mới
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

            // 🔹 3. Gọi PayOS
            long orderCode = long.Parse($"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}{payment.PaymentId}");
            var returnUrl = $"{_config["PayOS:ReturnUrl"]}?payment={payment.PaymentId}";
            var cancelUrl = $"{_config["PayOS:CancelUrl"]}?payment={payment.PaymentId}";
            var signature = GenerateSignature(orderCode, (int)amount, description, returnUrl, cancelUrl);

            var payload = new
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
                signature
            };

            var response = await _httpClient.PostAsJsonAsync("v2/payment-requests", payload);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"PayOS returned {response.StatusCode}: {responseBody}");

            var data = System.Text.Json.JsonSerializer.Deserialize<PayOSCreateResponse>(
                responseBody,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            var checkoutUrl = data?.Data?.CheckoutUrl ?? "";
            Console.WriteLine($"✅ Created PayOS link for order {orderId}: {checkoutUrl}");
            return checkoutUrl;
        }


        public async Task<string> CreateBookingPaymentAsync(int bookingId, int userId)
        {
            var booking = await _unitOfWork.BookingRepository.GetByIdAsync(bookingId);
            if (booking == null)
                return "Booking không tồn tại";
            if (booking.IsPaid)
                return "Booking này đã được thanh toán!";
            string description = $"Thanh toán lịch học #{bookingId}";
            string itemName = $"Lịch học #{bookingId}";

            // Chỉ cần gọi CreatePaymentUrlAsync để tạo Payment record và lấy link
            return await CreatePaymentUrlAsync(userId, bookingId, booking.TotalAmount ?? 0, description, itemName, 1);
        }

        public async Task<string> CreateCoursePaymentAsync(int courseId, int studentId, int userId)
        {
            // 🔹 1. Kiểm tra khóa học tồn tại
            var course = await _unitOfWork.CourseRepository.GetByIdAsync(courseId);
            if (course == null)
                return "Khóa học không tồn tại.";

            // 🔹 2. Tìm Enrollment hiện có của học viên cho khóa học này
            var enrollment = await _unitOfWork.EnrollmentRepository.FindAsync(
                e => e.CourseId == courseId && e.StudentId == studentId
            );

            // 🔹 3. Nếu đã có Enrollment
            if (enrollment != null)
            {
                // Nếu đã thanh toán thành công → không cho tạo lại
                if (enrollment.Status == StatusEnum.Completed.ToString() || enrollment.Status == StatusEnum.Success.ToString())
                    return "Khóa học này đã được thanh toán thành công!";

                // Nếu đang chờ thanh toán → không cho tạo lại
                if (enrollment.Status == StatusEnum.Pending.ToString() ||
                    enrollment.Status == StatusEnum.OnHold.ToString() ||
                    enrollment.Status == StatusEnum.InProgress.ToString())
                {
                    return "Bạn đã có một giao dịch đang xử lý. Vui lòng hoàn tất hoặc đợi hết hạn.";
                }

                // 🔹 Nếu là Failed, Cancelled, Expired → cho phép thanh toán lại
                enrollment.Status = StatusEnum.OnHold.ToString();
                enrollment.IsDeleted = false;
                enrollment.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.EnrollmentRepository.UpdateAndSaveAsync(enrollment);
            }
            else
            {
                // 🔹 4. Nếu chưa có Enrollment → tạo mới
                enrollment = new Enrollment
                {
                    CourseId = courseId,
                    StudentId = studentId,
                    Status = StatusEnum.OnHold.ToString(),
                    IsDeleted = false,
                    UpdatedAt = DateTime.UtcNow
                };
                await _unitOfWork.EnrollmentRepository.AddAndSaveAsync(enrollment);
            }

            // 🔹 5. Tạo mô tả & thông tin thanh toán
            string description = $"Thanh toán khóa học #{courseId}";
            string itemName = course.Title;

            // 🔹 6. Gọi service tạo link thanh toán PayOS
            return await CreatePaymentUrlAsync(
                userId,
                enrollment.EnrollmentId,
                course.PricePerSession ?? 0,
                description,
                itemName,
                2 // 2 = loại thanh toán khóa học (ví dụ bạn dùng enum PaymentType)
            );
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

            var orderStr = data.OrderCode.ToString();

            int paymentId = int.Parse(orderStr.Substring(10));
            var payosStatus = data.Code?.ToUpperInvariant(); // Dùng "code" từ data

            // BƯỚC 1: Dùng orderCode để tìm lại bản ghi Payment
            var payment = await _unitOfWork.PaymentRepository.GetByIdAsync((int)paymentId);
            if (payment == null)
            {
                Console.WriteLine($"⚠️ Payment with OrderCode (PaymentId) {paymentId} not found.");
                return;
            }

            // Nếu giao dịch đã xử lý rồi thì bỏ qua
            if (payment.StatusId != (int)StatusEnum.Pending)
            {
                Console.WriteLine($"ℹ️ Payment {paymentId} has already been processed.");
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
                        booking.IsPaid = true;
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
                        enrollment.Status = StatusEnum.Success.ToString();
                        enrollment.UpdatedAt = DateTime.UtcNow;
                    }
                    else
                    {
                        enrollment.Status = StatusEnum.Failed.ToString();
                        enrollment.DeletedAt = DateTime.UtcNow;
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
            Console.WriteLine($"✅ Webhook processed successfully for PaymentId {paymentId}. New status: {payment.StatusId}. Transaction created.");
        }
    }
}