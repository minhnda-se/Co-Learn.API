using CoLearn.Domain.DTOs;
using CoLearn.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoLearn.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubmissionController : ControllerBase
    {
        private readonly ISubmissionService _submissionService;

        public SubmissionController(ISubmissionService submissionService)
        {
            _submissionService = submissionService;
        }

        /// <summary>
        /// Sinh viên nộp bài cho 1 assignment
        /// </summary>
        [HttpPost("assignments/{assignmentId}/submissions")]
        public async Task<IActionResult> Create(int assignmentId, [FromBody] SubmissionRequestDto dto)
        {
            var result = await _submissionService.CreateAsync(assignmentId, dto);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Lấy danh sách submissions theo assignment
        /// </summary>
        [HttpGet("assignments/{assignmentId}/submissions")]
        public async Task<IActionResult> GetByAssignmentId(int assignmentId)
        {
            var result = await _submissionService.GetByAssignmentAsync(assignmentId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Lấy danh sách submissions theo student
        /// </summary>
        [HttpGet("students/{studentId}/submissions")]
        public async Task<IActionResult> GetByStudentId(int studentId)
        {
            var result = await _submissionService.GetByStudentAsync(studentId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Giáo viên chấm điểm + feedback cho submission
        /// </summary>
        [HttpPut("submissions/{id}/feedback")]
        public async Task<IActionResult> GiveFeedback(long id, [FromBody] SubmissionFeedbackRequestDto dto)
        {
            var result = await _submissionService.GiveFeedbackAsync(id, dto);
            return StatusCode(result.StatusCode, result);
        }
    }
}
