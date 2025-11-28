using CoLearn.Domain.DTOs;
using CoLearn.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using static CoLearn.Domain.DTOs.EnrollmentDtos;

namespace CoLearn.API.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        // GET: api/enrollments?pageIndex=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _enrollmentService.GetAllAsync(pageIndex, pageSize);
            return Ok(result.Value);
        }

        // GET: api/enrollments/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _enrollmentService.GetByIdAsync(id);
            return Ok(result.Value);
        }

        // GET: api/enrollments/student/{studentId}?pageIndex=1&pageSize=10
        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetByStudentId(int studentId, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _enrollmentService.GetByStudentIdAsync(studentId, pageIndex, pageSize);
            return Ok(result.Value);
        }

        // GET: api/enrollments/course/{courseId}?pageIndex=1&pageSize=10
        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetByCourseId(int courseId, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _enrollmentService.GetByCourseIdAsync(courseId, pageIndex, pageSize);
            return Ok(result.Value);
        }

        // POST: api/enrollments
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EnrollmentRequestDto dto)
        {
            if (dto == null) return BadRequest("Request body is null");

            var id = await _enrollmentService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        // PUT: api/enrollments/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EnrollmentRequestDto dto)
        {
            if (dto == null) return BadRequest("Request body is null");

            var result = await _enrollmentService.UpdateAsync(id, dto);
            if (result == -1) return NotFound();
            return NoContent();
        }

        // DELETE: api/enrollments/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _enrollmentService.DeleteAsync(id);
            if (result == -1) return NotFound();
            return NoContent();
        }
    }
}
