using CoLearn.Domain.DTOs;
using CoLearn.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoLearn.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public async Task<IActionResult> Create([FromBody] ScheduleRequestDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Xem lịch của giáo viên (Teacher/Student)
        /// </summary>
        [HttpGet("teacher/{teacherId}")]
        public async Task<IActionResult> GetByTeacher(int teacherId)
        {
            var result = await _service.GetByTeacherIdAsync(teacherId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Chỉnh sửa buổi học (Teacher)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ScheduleRequestDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Hủy lịch (Teacher/Admin)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}
