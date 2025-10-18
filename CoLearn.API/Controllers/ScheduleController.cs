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

        public ScheduleController(IScheduleService service)
        {
            _service = service;
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
    }
}
