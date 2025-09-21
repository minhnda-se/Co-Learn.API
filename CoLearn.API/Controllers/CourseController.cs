using CoLearn.Domain.DTOs;
using CoLearn.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoLearn.API.Controllers
{
    [Route("api/[controller]")] // => /api/course
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;
        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        // POST /api/course
        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] CourseRequestDto course)
        {
            int result = await _courseService.CreateAsync(course);
            if (result > 0)
                return CreatedAtAction(nameof(GetCourseById), new { id = result }, course);
            return BadRequest(new { message = "Create course failed" });
        }

        // PUT /api/course/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] CourseRequestDto course)
        {
            int result = await _courseService.UpdateAsync(id, course);
            if (result > 0)
                return Ok(new { message = "Update course successfully" });
            return NotFound(new { message = "Course not found" });
        }

        // DELETE /api/course/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            int result = await _courseService.DeleteAsync(id);
            if (result > 0)
                return Ok(new { message = "Delete course successfully" });
            return NotFound(new { message = "Course not found" });
        }

        // GET /api/course/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourseById(int id)
        {
            var course = await _courseService.GetByIdAsync(id);
            if (course == null)
                return NotFound(new { message = "Course not found" });
            return Ok(course);
        }

        // GET /api/course/search?keyword=...&teacherName=...
        [HttpGet("search")]
        public async Task<IActionResult> SearchCourses([FromQuery] string? keyword, [FromQuery] string? teacherName)
        {
            var courses = await _courseService.SearchCoursesAsync(keyword, teacherName);
            return Ok(courses);
        }

        //PUT /api/course/{id}/active?isActive=true/false
        [HttpPut("{id}/active")]
        public async Task<IActionResult> SetCourseActive(int id, [FromQuery] bool? isActive)
        {
            int result = await _courseService.SetCourseActive(id, isActive);
            if (result > 0)
                return Ok(new { message = "Set course active status successfully" });
            return NotFound(new { message = "Course not found" });
        }
    }
}
