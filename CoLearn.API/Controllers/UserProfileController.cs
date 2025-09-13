using CoLearn.Domain.DTOs;
using CoLearn.Domain.Interfaces.Services;

using CoLearn.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Eventing.Reader;
using System.Threading.Tasks;

namespace CoLearn.API
{
    [Route("api/profile")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;
        private readonly IStudentService _studentService;
        private readonly IParentService _parentService;
        private readonly ITeacherService _teacherService;

        public UserProfileController(IUserProfileService userProfileService, IStudentService studentService, ITeacherService teacherService, IParentService parentService)
        {
            _userProfileService = userProfileService;
            _teacherService = teacherService;
            _studentService = studentService;
            _parentService = parentService;
        }

        #region Account Profile
        // GET: api/profile/account
        [HttpGet("account")]
        public async Task<IActionResult> GetAllAccountProfiles()
        {
            var users = await _userProfileService.GetAllProfilesAsync();
            return Ok(users);
        }

        // GET: api/profile/account/{id}
        [HttpGet("account/{id}")]
        public async Task<IActionResult> GetAccountProfileById(int id)
        {
            var user = await _userProfileService.GetUserProfileAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }
        //POST: api/profile/account/create
        [HttpPost("account/create")]
        public async Task<IActionResult> CreateAccountProfile([FromBody] UserProfileDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new { message = "Invalid request data" });
            }

            // Map DTO -> Entity
            var profile = new UserProfile
            {
                UserId = dto.UserId,
                AvatarUrl = dto.AvatarUrl,
                Bio = dto.Bio,
                Address = dto.Address,
                ExtraJson = dto.ExtraJson,
                UpdatedAt = DateTime.UtcNow
            };

            int result = await _userProfileService.CreateUserProfielAsync(profile);

            if (result > 0)
            {
                return CreatedAtAction(
                    nameof(GetAccountProfileById),
                    new { id = profile.ProfileId },
                    new { message = "Create successfully", profile }
                );
            }

            return BadRequest(new { message = "Failed to create user profile" });
        }
        #endregion

        #region Student Profile
        // GET: api/profile/student
        [HttpGet("student")]
        public async Task<IActionResult> GetAllStudentProfiles()
        {
            var users = await _studentService.GetAllStudentsAsync();
            return Ok(users);
        }

        // GET: api/profile/student/{id}
        [HttpGet("student/{id}")]
        public async Task<IActionResult> GetStudentProfileById(int id)
        {
            var user = await _studentService.GetStudentByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }
        #endregion

        #region Teacher Profile
        // GET: api/profile/teacher
        [HttpGet("teacher")]
        public async Task<IActionResult> GetAllTeacherProfiles()
        {
            var teachers = await _teacherService.GetAllTeachersAsync();
            return Ok(teachers);
        }

        // GET: api/profile/teacher/{id}
        [HttpGet("teacher/{id}")]
        public async Task<IActionResult> GetTeacherProfileById(int id)
        {
            var teacher = await _teacherService.GetTeacherByIdAsync(id);
            if (teacher == null)
                return NotFound();

            return Ok(teacher);
        }
        #endregion

        #region Parent Profile
        // GET: api/profile/parent
        [HttpGet("parent")]
        public async Task<IActionResult> GetAllParentProfiles()
        {
            var parents = await _parentService.GetAllParentsAsync();
            return Ok(parents);
        }

        // GET: api/profile/teacher/{id}
        [HttpGet("parent/{id}")]
        public async Task<IActionResult> GetParentProfileById(int id)
        {
            var parent = await _parentService.GetParentByIdAsync(id);
            if (parent == null)
                return NotFound();

            return Ok(parent);
        }
        #endregion
    }
}
