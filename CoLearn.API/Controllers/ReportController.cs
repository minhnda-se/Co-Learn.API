using CoLearn.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoLearn.API.Controllers
{
    [Authorize]
    [Route("api/report")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IEarningService _earningService;
        private readonly decimal systemFeeRate = 20;

        public ReportController(IEarningService earningSerive)
        {
            _earningService = earningSerive;
        }

        /// <summary>
        /// Get all teacher earnings summary
        /// </summary>
        [Authorize(Roles = "4")]
        [HttpGet("earning/teachers")]
        public async Task<IActionResult> GetAllTeacherEarningsAsync(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var result = await _earningService.GetAllTeacherEarningsAsync(startDate, endDate);
            if (result == null)
                return NotFound(new { Message = "Teacher not found or no earnings data." });
            var totalEarnings = result.Sum(e => e.TotalEarnings);
            var totalSystemFees = totalEarnings * systemFeeRate / 100;
            var totalRevenue = totalEarnings - totalSystemFees;
            var totalTeacher = result.Count;
            return Ok(new { totalTeachersEarning  = totalEarnings, totalSystemEarnings = totalSystemFees, totalTeachersRevenue = totalRevenue , totalTeachers = totalTeacher, teacherEarnings = result });
        }

        /// <summary>
        /// Get teacher earnings summary
        /// </summary>
        [Authorize(Roles = "3,4")]
        [HttpGet("earning/teachers/{teacherId}")]
        public async Task<IActionResult> GetTeacherEarningsByTeacherIdAsync(
            int teacherId,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var result = await _earningService.GetTeacherEarningsByTeacherIdAsync(teacherId, startDate, endDate);
            if (result == null)
                return NotFound(new { Message = "Teacher not found or no earnings data." });

            return Ok(result);
        }

        [Authorize(Roles = "4")]
        [HttpGet("teacher-revenue-trends")]
        public async Task<IActionResult> GetRevenueTrends([FromQuery] int? teacherId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] string groupBy = "month")
        {
            var result = await _earningService.GetTeacherRevenueTrendsAsync(teacherId, startDate, endDate, groupBy);
            return Ok(result);
        }

        [Authorize(Roles = "4")]
        [HttpGet("top-teachers")]
        public async Task<IActionResult> GetTopTeachers([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] int limit = 10)
        {
            var result = await _earningService.GetTopTeachersAsync(startDate, endDate, limit);
            return Ok(result);
        }
    }
}
