using CoLearn.Domain.DTOs;
using CoLearn.Domain.Enums;
using CoLearn.Domain.Interfaces;
using CoLearn.Domain.Interfaces.Services;
using CoLearn.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoLearn.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPayOSService _payOSService;
        public PaymentController(IPaymentService paymentService, IPayOSService payOSService, IUnitOfWork unitOfWork)
        {
            _paymentService = paymentService;
            _payOSService = payOSService;
            _unitOfWork = unitOfWork;

        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRequest dto)
        {
            var result = await _paymentService.CreateAsync(dto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("vnpay-return")]
        public async Task<IActionResult> HandleVnPayReturn()
        {
            var query = Request.Query;
            var result = await _paymentService.HandleVnPayReturnAsync(query);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _paymentService.GetAllAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _paymentService.GetByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("payos/booking/{bookingId}/user/{userId}")]
        public async Task<IActionResult> CreateBookingPayment(int bookingId, int userId)
        {
            var url = await _payOSService.CreateBookingPaymentAsync(bookingId, userId);
            return Ok(new { checkoutUrl = url });
        }

        [HttpPost("payos/course/{courseId}/student/{studentId}/user/{userId}")]
        public async Task<IActionResult> CreateCoursePayment(int courseId, int studentId, int userId)
        {
            var url = await _payOSService.CreateCoursePaymentAsync(courseId, studentId, userId);
            return Ok(new { checkoutUrl = url });
        }

        [HttpPost("payos/webhook")]
        public async Task<IActionResult> Webhook([FromBody] PayOSWebhookPayload payload)
        {
            await _payOSService.HandleWebhookAsync(payload);
            return Ok(new { message = "Webhook processed successfully" });
        }

        [HttpGet("payos/cancel")]
        public async Task<IActionResult> CancelPayment([FromQuery] long payment)
        {
            // 🔍 Tìm payment theo PaymentId
            var paymentEntity = await _unitOfWork.PaymentRepository.GetByIdAsync(payment);
            if (paymentEntity == null)
                return NotFound(new { message = "Payment not found" });

            // 🟡 Nếu payment đang chờ => đổi trạng thái sang Cancelled
            if (paymentEntity.StatusId == (int)StatusEnum.Pending)
            {
                paymentEntity.StatusId = (int)StatusEnum.Cancelled;
                paymentEntity.UpdatedAt = DateTime.UtcNow;
            }

            // 🧩 Nếu là Enrollment Payment => cập nhật Enrollment tương ứng
            if (paymentEntity.EnrollmentId.HasValue)
            {
                var enrollment = await _unitOfWork.EnrollmentRepository.GetByIdAsync(paymentEntity.EnrollmentId.Value);
                if (enrollment != null && enrollment.Status == StatusEnum.OnHold.ToString())
                {
                    enrollment.Status = StatusEnum.Cancelled.ToString();
                    enrollment.DeletedAt = DateTime.UtcNow;
                    enrollment.IsDeleted = true;
                }
            }

            await _unitOfWork.CommitAsync(); // 🔥 commit tất cả thay đổi

            var result = await _paymentService.GetByIdAsync(payment);
            return StatusCode(result.StatusCode, result);
        }


    }
}
