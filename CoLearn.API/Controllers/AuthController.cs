using CoLearn.Domain.DTOs.Request;
using CoLearn.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoLearn.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRequest.CreateUserModel dto)
        {
            var result = await _authService.RegisterAsync(dto);
            return result.StatusCode == 200 ? Ok(result) : BadRequest(result);
        }

        [HttpGet("verify")]
        public async Task<IActionResult> Verify([FromQuery] string token)
        {
            var result = await _authService.VerifyEmailAsync(token);
            return result.StatusCode == 200 ? Ok("Your email has been successfully verified 🎉. You can login now!!") : BadRequest(result);
        }
    }

}
