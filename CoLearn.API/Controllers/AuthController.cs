using CoLearn.Domain.DTOs;
using CoLearn.Domain.DTOs.Request;
using CoLearn.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<IActionResult> Verify([FromQuery] int type, string token)
        {
            var result = await _authService.VerifyEmailAsync(type, token);
            return result.StatusCode == 200 ? Ok("Your email has been successfully verified 🎉. You can login now!!") : BadRequest(result.Value);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto dto)
        {
            if (dto.NewPassword != dto.ConfirmPassword)
                return BadRequest(new { message = "Passwords do not match." });

            var userId = int.Parse(User.FindFirst("id")!.Value);
            var result = await _authService.ChangePasswordAsync(userId, dto.CurrentPassword, dto.NewPassword);

            if (result.StatusCode != 200)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }
    }

}
