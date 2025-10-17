using CoLearn.Domain.DTOs;
using CoLearn.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using static CoLearn.Domain.DTOs.BookingDtos;

namespace CoLearn.API.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    [Authorize] // ✅ chỉ cho user đăng nhập
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // ==================== GET ====================

        // ✅ Admin + Teacher + Parent có thể xem chi tiết booking
        [HttpGet("{id}")]
        [Authorize(Roles = "2,3,4")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _bookingService.GetByIdAsync(id);
            if (result.Value == null)
                return StatusCode(result.StatusCode, result.Message);

            return Ok(result.Value);
        }

        // ✅ Admin có thể xem tất cả booking
        [HttpGet]
        [Authorize(Roles = "4")]
        public async Task<IActionResult> GetAll([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _bookingService.GetAllAsync(pageIndex, pageSize);
            return StatusCode(result.StatusCode, result.Value);
        }

        // ✅ Student xem booking của chính mình
        [HttpGet("student/{studentId}")]
        [Authorize(Roles = "1,2 ")]
        public async Task<IActionResult> GetByStudentId(int studentId, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _bookingService.GetByStudentIdAsync(studentId, pageIndex, pageSize);
            return StatusCode(result.StatusCode, result.Value);
        }

        // ✅ Parent xem booking của con mình
        [HttpGet("parent/{parentId}")]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> GetByParentId(int parentId, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _bookingService.GetByParentIdAsync(parentId, pageIndex, pageSize);
            return StatusCode(result.StatusCode, result.Value);
        }

        //// ✅ Teacher xem booking theo schedule hoặc của mình
        //[HttpGet("schedule/{scheduleId}")]
        //[Authorize(Roles = "3,4")]
        //public async Task<IActionResult> GetByScheduleId(int scheduleId, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        //{
        //    var result = await _bookingService.GetByScheduleIdAsync(scheduleId, pageIndex, pageSize);
        //    return StatusCode(result.StatusCode, result.Value);
        //}

        [HttpGet("teacher/{teacherId}")]
        [Authorize(Roles = "3,4")]
        public async Task<IActionResult> GetByTeacherId(int teacherId, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _bookingService.GetByTeacherIdAsync(teacherId, pageIndex, pageSize);
            return StatusCode(result.StatusCode, result.Value);
        }

        [HttpGet("status/{statusId}")]
        [Authorize]
        public async Task<IActionResult> GetByStatusId(int statusId, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _bookingService.GetByStatusIdAsync(statusId, pageIndex, pageSize);
            return StatusCode(result.StatusCode, result.Value);
        }

        // ==================== POST ====================

        // ✅ Student hoặc Parent có thể tạo booking mới
        [HttpPost]
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> Create([FromBody] BookingRequestDto dto)
        {
            if (dto == null) return BadRequest("Request body is null");

            var id = await _bookingService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        // ==================== PUT ====================

        // ✅ Student, Parent có thể update trước khi teacher confirm
        [HttpPut("{id}")]
        [Authorize(Roles = "1,2,4")]
        public async Task<IActionResult> Update(int id, [FromBody] BookingRequestDto dto)
        {
            if (dto == null) return BadRequest("Request body is null");

            var result = await _bookingService.UpdateAsync(id, dto);
            if (result == -1) return NotFound();

            return NoContent();
        }

        // ✅ Admin hoặc Teacher có thể cập nhật trạng thái booking
        [HttpPut("{id}/status")]
        [Authorize(Roles = "3,4")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] int statusId)
        {
            var booking = await _bookingService.GetByIdAsync(id);
            if (booking.Value == null) return NotFound();
            var result = await _bookingService.UpdateStatusAsync(id, statusId);
            if (result == -1) return NotFound();

            return NoContent();
        }

        // ==================== DELETE ====================

        // ✅ Admin hoặc Parent có thể xóa booking
        [HttpDelete("{id}")]
        [Authorize(Roles = "2,4")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _bookingService.DeleteAsync(id);
            if (result == -1) return NotFound();

            return NoContent();
        }

        // ==================== ACTIONS ====================

        // ✅ Teacher hoặc Admin confirm
        [HttpPost("{id}/confirm")]
        [Authorize(Roles = "3,4")]
        public async Task<IActionResult> ConfirmBooking(int id)
        {
            var result = await _bookingService.ConfirmBookingAsync(id);
            if (result.StatusCode != 200)
                return BadRequest(result.Message);

            return Ok(result);
        }

        // ✅ Teacher hoặc Admin decline
        [HttpPost("{id}/decline")]
        [Authorize(Roles = "3,4")]
        public async Task<IActionResult> DeclineBooking(int id)
        {
            var result = await _bookingService.DeclineBookingAsync(id);
            if (result.StatusCode != 200)
                return BadRequest(result.Message);

            return Ok(result);
        }
    }
}
