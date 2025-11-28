using CoLearn.Domain.DTOs;
using CoLearn.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoLearn.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class CourseMaterialController : ControllerBase
    {
        private readonly ICourseMaterialService _materialService;

        public CourseMaterialController(ICourseMaterialService materialService)
        {
            _materialService = materialService;
        }

        // POST /api/lessons/{lessonId}/materials
        [HttpPost("lessons/{lessonId}/materials")]
        public async Task<IActionResult> Create(int lessonId, [FromBody] CourseMaterialRequestDto dto)
        {
            var id = await _materialService.CreateAsync(lessonId, dto);
            return CreatedAtAction(nameof(GetByLesson), new { lessonId }, new { MaterialId = id });
        }

        // PUT /api/materials/{id}
        [HttpPut("materials/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CourseMaterialRequestDto dto)
        {
            var result = await _materialService.UpdateAsync(id, dto);
            if (result == 0) return NotFound();
            return NoContent();
        }

        // DELETE /api/materials/{id}
        [HttpDelete("materials/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _materialService.DeleteAsync(id);
            if (result == 0) return NotFound();
            return NoContent();
        }

        // GET /api/lessons/{lessonId}/materials
        [HttpGet("lessons/{lessonId}/materials")]
        public async Task<IActionResult> GetByLesson(int lessonId, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var materials = await _materialService.GetByLessonIdAsync(pageIndex, pageSize, lessonId);
            return StatusCode(materials.StatusCode, materials);
        }

        // GET /api/materials/{id}
        [HttpGet("materials/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var material = await _materialService.GetByIdAsync(id);
            return StatusCode(material.StatusCode, material);
        }
    }
}
