using CoLearn.Domain.DTOs;
using CoLearn.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using static CoLearn.Domain.DTOs.BookingDtos;

namespace CoLearn.API.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // GET: api/bookings/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _bookingService.GetByIdAsync(id);
            if (result.Value == null)
                return StatusCode(result.StatusCode, result.Message);

            return Ok(result.Value);
        }

        // GET: api/bookings?pageIndex=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _bookingService.GetAllAsync(pageIndex, pageSize);
            return StatusCode(result.StatusCode, result.Value);
        }

        // GET: api/bookings/student/{studentId}?pageIndex=1&pageSize=10
        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetByStudentId(int studentId, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _bookingService.GetByStudentIdAsync(studentId, pageIndex, pageSize);
            return StatusCode(result.StatusCode, result.Value);
        }
        // GET: api/bookings/parent/{parentId}?pageIndex=1&pageSize=10
        [HttpGet("parent/{parentId}")]
        public async Task<IActionResult> GetByParentId(int parentId, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _bookingService.GetByParentIdAsync(parentId, pageIndex, pageSize);
            return StatusCode(result.StatusCode, result.Value);
        }

        // GET: api/bookings/schedule/{scheduleId}?pageIndex=1&pageSize=10
        [HttpGet("schedule/{scheduleId}")]
        public async Task<IActionResult> GetByScheduleId(int scheduleId, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _bookingService.GetByScheduleIdAsync(scheduleId, pageIndex, pageSize);
            return StatusCode(result.StatusCode, result.Value);
        }

        // GET: api/bookings/schedule/{scheduleId}?pageIndex=1&pageSize=10
        [HttpGet("teacher/{teacherId}")]
        public async Task<IActionResult> GetByTeacherId(int teacherId, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _bookingService.GetByScheduleIdAsync(teacherId, pageIndex, pageSize);
            return StatusCode(result.StatusCode, result.Value);
        }

        // GET: api/bookings/status/{statusId}?pageIndex=1&pageSize=10
        [HttpGet("status/{statusId}")]
        public async Task<IActionResult> GetByStatusId(int statusId, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _bookingService.GetByStatusIdAsync(statusId, pageIndex, pageSize);
            return StatusCode(result.StatusCode, result.Value);
        }

        // POST: api/bookings
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookingRequestDto dto)
        {
            if (dto == null) return BadRequest("Request body is null");

            var id = await _bookingService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        // PUT: api/bookings/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BookingRequestDto dto)
        {
            if (dto == null) return BadRequest("Request body is null");

            var result = await _bookingService.UpdateAsync(id, dto);
            if (result == -1) return NotFound();

            return NoContent();
        }

        // PUT: api/bookings/{id}/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] int statusId)
        {
            var booking = await _bookingService.GetByIdAsync(id);
            if (booking.Value == null) return NotFound();
            var result = await _bookingService.UpdateStatusAsync(id, statusId);
            if (result == -1) return NotFound();

            return NoContent();
        }

        // DELETE: api/bookings/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _bookingService.DeleteAsync(id);
            if (result == -1) return NotFound();

            return NoContent();
        }
    }
}
