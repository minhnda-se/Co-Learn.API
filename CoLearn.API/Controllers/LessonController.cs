using CoLearn.Domain.DTOs;
using CoLearn.Domain.DTOs.Requests;
using CoLearn.Domain.DTOs.Responses;
using CoLearn.Domain.Interfaces.Services;
using CoLearn.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoLearn.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class LessonController : ControllerBase
    {
        private readonly ILessonService _lessonService;

        public LessonController(ILessonService lessonService)
        {
            _lessonService = lessonService;
        }

        /// <summary>
        /// Tạo lesson trong 1 course
        /// </summary>
        [HttpPost("courses/{courseId}/lessons")]
        public async Task<IActionResult> Create(int courseId, [FromBody] LessonRequestDto dto)
        {
            try
            {
                var id = await _lessonService.CreateAsync(courseId, dto);
                return CreatedAtAction(nameof(GetByCourseId), new { courseId = courseId }, new { LessonId = id });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Sửa lesson
        /// </summary>
        [HttpPut("lessons/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] LessonRequestDto dto)
        {
            try
            {
                var result = await _lessonService.UpdateAsync(id, dto);
                if (result == 0) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Xóa lesson (soft delete)
        /// </summary>
        [HttpDelete("lessons/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _lessonService.DeleteAsync(id);
                if (result == 0) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lấy danh sách Lesson theo CourseId (phân trang)
        /// </summary>
        [HttpGet("courses/{courseId}/lessons")]
        public async Task<IActionResult> GetByCourseId(int courseId, int pageIndex = 1, int pageSize = 10)
        {
            var result = await _lessonService.GetByCourseIdAsync(pageIndex, pageSize, courseId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>  
        /// Lấy danh sách Lesson theo LessonId
        /// </summary>
        [HttpGet("lessons/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _lessonService.GetByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}
