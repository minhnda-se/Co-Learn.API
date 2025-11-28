using CoLearn.Domain.DTOs;
using CoLearn.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoLearn.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // yêu cầu đăng nhập cho toàn controller
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _service;
        private readonly IBookingService bookingService;

        public ScheduleController(IScheduleService service, IBookingService bookingService)
        {
            _service = service;
            this.bookingService = bookingService;
        }

        /// <summary>
        /// Tạo schedule (Teacher)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "3")] // Teacher
        public async Task<IActionResult> Create([FromBody] ScheduleRequestDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Xem lịch của giáo viên (Student, Teacher, Admin)
        /// </summary>
        [HttpGet("teacher/{teacherId}")]
        [Authorize(Roles = "1,3,4")] // Student, Teacher, Admin
        public async Task<IActionResult> GetByTeacher(int teacherId)
        {
            var result = await _service.GetByTeacherIdAsync(teacherId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Xem lịch của student  (Student, Teacher, Admin)
        /// </summary>
        [HttpGet("student/{studentId}")]
        [Authorize(Roles = "1,3,4")] // Student, Teacher, Admin
        public async Task<IActionResult> GetByStudent(int studentId)
        {
            var result = await _service.GetByStudentIdAsync(studentId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Xem lịch của theo id (Student, Teacher, Admin)
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "1,3,4")] // Student, Teacher, Admin
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Chỉnh sửa buổi học (Teacher)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "3")] // Teacher
        public async Task<IActionResult> Update(int id, [FromBody] ScheduleRequestDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Hủy lịch (Teacher/Admin)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "3,4")] // Teacher, Admin
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Cập nhật meeting link cho buổi học (Teacher/Admin)
        /// </summary>
        [HttpPut("meeting/{id}")]
        [Authorize(Roles = "3,4")] // Teacher, Admin
        public async Task<IActionResult> UpdateMeetingLink(int id, [FromBody] string meetingLink)
        {
            var result = await _service.UpdateMeetingLink(id, meetingLink);
            return StatusCode(result.StatusCode, result);
        }

    }
}
