using CoLearn.Domain.Common;
using CoLearn.Domain.DTOs.Request;
using CoLearn.Domain.Interfaces.Services;
using CoLearn.Domain.Models;
using CoLearn.Services.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CoLearn.API.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "3,4")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet]
        [Authorize(Roles = "4")]
        public async Task<IActionResult> GetAll([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var users = await _service.GetAllAsync(pageIndex, pageSize);
            return StatusCode(users.StatusCode, users);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] UserRequest.CreateUserModel dto)
        {
            var created = await _service.CreateAsync(dto);
            return StatusCode(created.StatusCode, created);
        }

        [HttpPost("unban/{id}")]
        [Authorize(Roles = "4")]
        public async Task<IActionResult> Unban(int id)
        {
            var result = await _service.UnbanAsync(id);
            return StatusCode(result.StatusCode, result);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserRequest.UpdateUserModel dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "4")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserRequest.LoginRequest request)
        {
            var result = await _service.LoginAsync(request);
            return StatusCode(result.StatusCode, result);
        }
    }
}
