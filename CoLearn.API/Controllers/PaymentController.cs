using CoLearn.Domain.DTOs;
using CoLearn.Domain.Interfaces;
using CoLearn.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoLearn.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IPayOSService _payOSService;
        public PaymentController(IPaymentService paymentService, IPayOSService payOSService)
        {
            _paymentService = paymentService;
            _payOSService = payOSService;

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
    }
}
