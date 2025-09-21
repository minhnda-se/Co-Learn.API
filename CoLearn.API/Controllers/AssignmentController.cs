using CoLearn.Domain.DTOs;
using CoLearn.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoLearn.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class AssignmentController : ControllerBase
    {
        private readonly IAssignmentService _assignmentService;

        public AssignmentController(IAssignmentService assignmentService)
        {
            _assignmentService = assignmentService;
        }

        /// <summary>
        /// Tạo assignment mới cho 1 lesson
        /// </summary>
        [HttpPost("lessons/{lessonId}/assignments")]
        public async Task<IActionResult> Create(int lessonId, [FromBody] AssignmentRequestDto dto)
        {
            try
            {
                var id = await _assignmentService.CreateAsync(lessonId, dto);
                return CreatedAtAction(nameof(GetByLesson), new { lessonId = lessonId }, dto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Cập nhật assignment
        /// </summary>
        [HttpPut("assignments/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AssignmentRequestDto dto)
        {
            try
            {
                var result = await _assignmentService.UpdateAsync(id, dto);
                if (result == 0) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Xóa assignment (soft delete)
        /// </summary>
        [HttpDelete("assignments/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _assignmentService.DeleteAsync(id);
                if (result == 0) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lấy danh sách assignment theo lesson
        /// </summary>
        [HttpGet("lessons/{lessonId}/assignments")]
        public async Task<ActionResult<List<AssignmentResponseDto>>> GetByLesson(int lessonId)
        {
            try
            {
                var result = await _assignmentService.GetByLessonIdAsync(lessonId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lấy danh sách submission theo assignment
        /// </summary>
        [HttpGet("assignments/{id}/submissions") ]
        public async Task<IActionResult> GetSubmissions(int id)
        {
            var result = await _assignmentService.GetSubmissionsByAssignmentIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Feedback cho submission
        /// </summary>
        [HttpPut("submissions/{id}/feedback")]
        public async Task<IActionResult> UpdateFeedback(long id, [FromBody] SubmissionFeedbackRequestDto dto)
        {
            var result = await _assignmentService.UpdateFeedbackAsync(id, dto);
            if (result == null) return NotFound(new { message = "Submission not found" });

            return Ok(result);
        }
    }
}