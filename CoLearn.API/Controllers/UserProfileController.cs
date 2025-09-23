using CoLearn.Domain.DTOs;
using CoLearn.Domain.DTOs.Requests;
using CoLearn.Domain.DTOs.Responses;
using CoLearn.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

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

        public UserProfileController(
            IUserProfileService userProfileService,
            IStudentService studentService,
            ITeacherService teacherService,
            IParentService parentService)
        {
            _userProfileService = userProfileService;
            _teacherService = teacherService;
            _studentService = studentService;
            _parentService = parentService;
        }

        #region Account Profile
        [HttpGet("account")]
        public async Task<IActionResult> GetAllAccountProfiles()
        {
            var users = await _userProfileService.GetAllProfilesAsync();
            return Ok(users);
        }

        [HttpGet("account/{id}")]
        public async Task<IActionResult> GetAccountProfileById(int id)
        {
            var user = await _userProfileService.GetUserProfileAsync(id);
            if (user == null) return NotFound(new { message = "User profile not found" });

            return Ok(user);
        }

        [HttpPost("account")]
        public async Task<IActionResult> CreateAccountProfile([FromBody] UserProfileDto dto)
        {
            if (dto == null) return BadRequest(new { message = "Invalid request data" });

            var result = await _userProfileService.CreateUserProfileAsync(dto);

            if (result > 0)
                return CreatedAtAction(nameof(GetAccountProfileById), new { id = dto.UserId }, dto);

            return BadRequest(new { message = "Failed to create user profile" });
        }

        [HttpPut("account/{id}")]
        public async Task<IActionResult> UpdateAccountProfile(int id, [FromBody] UserProfileDto dto)
        {
            if (dto == null || id != dto.UserId)
                return BadRequest(new { message = "Invalid request data" });

            var result = await _userProfileService.UpdateUserProfileAsync(dto);

            if (result > 0) return Ok(new { message = "Update successfully" });

            return NotFound(new { message = "User profile not found" });
        }

        [HttpDelete("account/{id}")]
        public async Task<IActionResult> DeleteAccountProfile(int id)
        {
            var result = await _userProfileService.DeleteUserProfileAsync(id);

            if (result > 0) return Ok(new { message = "Delete successfully" });

            return NotFound(new { message = "User profile not found" });
        }
        #endregion

        #region Student Profile
        [HttpGet("student")]
        public async Task<IActionResult> GetAllStudentProfiles()
        {
            var students = await _studentService.GetAllStudentsAsync();
            return Ok(students);
        }

        [HttpGet("student/{id}")]
        public async Task<IActionResult> GetStudentProfileById(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null) return NotFound(new { message = "Student profile not found" });

            return Ok(student);
        }

        [HttpPost("student")]
        public async Task<IActionResult> CreateStudentProfile([FromBody] StudentDtoRequest dto)
        {
            if (dto == null) return BadRequest(new { message = "Invalid request data" });

            var result = await _studentService.CreateStudentAsync(dto);

            if (result > 0)
                return CreatedAtAction(nameof(GetStudentProfileById), new { id = dto.UserId }, dto);

            return BadRequest(new { message = "Failed to create student profile" });
        }

        [HttpPut("student/{id}")]
        public async Task<IActionResult> UpdateStudentProfile(int id, [FromBody] StudentDtoRequest dto)
        {
            if (dto == null || id != dto.UserId)
                return BadRequest(new { message = "Invalid request data" });

            var result = await _studentService.UpdateStudentAsync(dto);

            if (result > 0) return Ok(new { message = "Update successfully" });

            return NotFound(new { message = "Student profile not found" });
        }

        [HttpDelete("student/{id}")]
        public async Task<IActionResult> DeleteStudentProfile(int id)
        {
            var result = await _studentService.DeleteStudentAsync(id);

            if (result > 0) return Ok(new { message = "Delete successfully" });

            return NotFound(new { message = "Student profile not found" });
        }
        #endregion

        #region Teacher Profile
        [HttpGet("teacher")]
        public async Task<IActionResult> GetAllTeacherProfiles()
        {
            var teachers = await _teacherService.GetAllTeachersAsync();
            return Ok(teachers);
        }

        [HttpGet("teacher/{id}")]
        public async Task<IActionResult> GetTeacherProfileById(int id)
        {
            var teacher = await _teacherService.GetTeacherByIdAsync(id);
            if (teacher == null) return NotFound(new { message = "Teacher profile not found" });

            return Ok(teacher);
        }

        [HttpPost("teacher")]
        public async Task<IActionResult> CreateTeacherProfile([FromBody] TeacherDtoRequest dto)
        {
            if (dto == null) return BadRequest(new { message = "Invalid request data" });

            var result = await _teacherService.CreateTeacherAsync(dto);

            if (result > 0)
                return CreatedAtAction(nameof(GetTeacherProfileById), new { id = dto.UserId }, dto);

            return BadRequest(new { message = "Failed to create teacher profile" });
        }

        [HttpPut("teacher/{id}")]
        public async Task<IActionResult> UpdateTeacherProfile(int id, [FromBody] TeacherDtoRequest dto)
        {
            if (dto == null || id != dto.UserId)
                return BadRequest(new { message = "Invalid request data" });

            var result = await _teacherService.UpdateTeacherAsync(dto);

            if (result > 0) return Ok(new { message = "Update successfully" });

            return NotFound(new { message = "Teacher profile not found" });
        }

        [HttpDelete("teacher/{id}")]
        public async Task<IActionResult> DeleteTeacherProfile(int id)
        {
            var result = await _teacherService.DeleteTeacherAsync(id);

            if (result > 0) return Ok(new { message = "Delete successfully" });

            return NotFound(new { message = "Teacher profile not found" });
        }
        #endregion

        #region Parent Profile
        [HttpGet("parent")]
        public async Task<IActionResult> GetAllParentProfiles()
        {
            var parents = await _parentService.GetAllParentsAsync();
            return Ok(parents);
        }

        [HttpGet("parent/{id}")]
        public async Task<IActionResult> GetParentProfileById(int id)
        {
            var parent = await _parentService.GetParentByIdAsync(id);
            if (parent == null) return NotFound(new { message = "Parent profile not found" });

            return Ok(parent);
        }

        [HttpPost("parent")]
        public async Task<IActionResult> CreateParentProfile([FromBody] ParentDtoRequest dto)
        {
            if (dto == null) return BadRequest(new { message = "Invalid request data" });

            var result = await _parentService.CreateParentAsync(dto);

            if (result > 0)
                return CreatedAtAction(nameof(GetParentProfileById), new { id = dto.UserId }, dto);

            return BadRequest(new { message = "Failed to create parent profile" });
        }

        [HttpPut("parent/{id}")]
        public async Task<IActionResult> UpdateParentProfile(int id, [FromBody] ParentDtoRequest dto)
        {
            if (dto == null || id != dto.UserId)
                return BadRequest(new { message = "Invalid request data" });

            var result = await _parentService.UpdateParentAsync(dto);

            if (result > 0) return Ok(new { message = "Update successfully" });

            return NotFound(new { message = "Parent profile not found" });
        }

        [HttpDelete("parent/{id}")]
        public async Task<IActionResult> DeleteParentProfile(int id)
        {
            var result = await _parentService.DeleteParentAsync(id);

            if (result > 0) return Ok(new { message = "Delete successfully" });

            return NotFound(new { message = "Parent profile not found" });
        }
        #endregion
    }
}
