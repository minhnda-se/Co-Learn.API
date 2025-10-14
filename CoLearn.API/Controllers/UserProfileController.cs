using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CoLearn.Domain.DTOs;
using CoLearn.Domain.DTOs.Requests;
using CoLearn.Domain.Interfaces.Services;

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
        // 🔒 Chỉ Admin mới có quyền xem & quản lý tất cả profile
        [Authorize(Roles = "4")]
        [HttpGet("account")]
        public async Task<IActionResult> GetAllAccountProfiles()
        {
            var users = await _userProfileService.GetAllProfilesAsync();
            return Ok(users);
        }

        // 🔒 Admin + chính chủ user có thể xem
        [Authorize(Roles = "1,2,3,4")]
        [HttpGet("account/{id}")]
        public async Task<IActionResult> GetAccountProfileById(int id)
        {
            var user = await _userProfileService.GetUserProfileAsync(id);
            if (user == null) return NotFound(new { message = "User profile not found" });
            return Ok(user);
        }

        [Authorize(Roles = "4")]
        [HttpPost("account")]
        public async Task<IActionResult> CreateAccountProfile([FromBody] UserProfileDto dto)
        {
            var result = await _userProfileService.CreateUserProfileAsync(dto);
            if (result > 0)
                return CreatedAtAction(nameof(GetAccountProfileById), new { id = dto.UserId }, dto);
            return BadRequest(new { message = "Failed to create user profile" });
        }

        [Authorize(Roles = "4")]
        [HttpPut("account/{id}")]
        public async Task<IActionResult> UpdateAccountProfile(int id, [FromBody] UserProfileDto dto)
        {
            if (dto == null || id != dto.UserId)
                return BadRequest(new { message = "Invalid request data" });

            var result = await _userProfileService.UpdateUserProfileAsync(dto);
            if (result > 0) return Ok(new { message = "Update successfully" });
            return NotFound(new { message = "User profile not found" });
        }

        [Authorize(Roles = "4")]
        [HttpDelete("account/{id}")]
        public async Task<IActionResult> DeleteAccountProfile(int id)
        {
            var result = await _userProfileService.DeleteUserProfileAsync(id);
            if (result > 0) return Ok(new { message = "Delete successfully" });
            return NotFound(new { message = "User profile not found" });
        }
        #endregion

        #region Student Profile
        [Authorize(Roles = "4")]
        [HttpGet("student")]
        public async Task<IActionResult> GetAllStudentProfiles()
        {
            var students = await _studentService.GetAllStudentsAsync();
            return Ok(students);
        }

        [Authorize(Roles = "1,2,4")] // Student tự xem, Parent xem con, Admin xem tất cả
        [HttpGet("student/{userId}")]
        public async Task<IActionResult> GetStudentProfileByUserId(int userId)
        {
            var student = await _studentService.GetStudentByUserIdAsync(userId);
            if (student == null) return NotFound(new { message = "Student profile not found" });
            return Ok(student);
        }

        [Authorize(Roles = "2,4")] // Parent hoặc Admin xem danh sách học sinh của parent
        [HttpGet("students/{parentId}")]
        public async Task<IActionResult> GetStudentsProfileByParnetId(int parentId)
        {
            var student = await _studentService.GetStudentsByParentIdAsync(parentId);
            if (student == null) return NotFound(new { message = "Student profile not found" });
            return Ok(student);
        }

        [Authorize(Roles = "4")] // Chỉ admin tạo mới
        [HttpPost("student")]
        public async Task<IActionResult> CreateStudentProfile([FromBody] StudentDtoRequest dto)
        {
            var result = await _studentService.CreateStudentAsync(dto);
            return result.StatusCode == 200 ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "1,4")] // Student cập nhật profile của mình hoặc admin
        [HttpPut("student/{id}")]
        public async Task<IActionResult> UpdateStudentProfile(int id, [FromBody] StudentDtoRequest dto)
        {
            var result = await _studentService.UpdateStudentAsync(id, dto);
            if (result > 0) return Ok(new { message = "Update successfully" });
            return NotFound(new { message = "Student profile not found" });
        }

        [Authorize(Roles = "4")]
        [HttpDelete("student/{id}")]
        public async Task<IActionResult> DeleteStudentProfile(int id)
        {
            var result = await _studentService.DeleteStudentAsync(id);
            if (result > 0) return Ok(new { message = "Delete successfully" });
            return NotFound(new { message = "Student profile not found" });
        }
        #endregion

        #region Teacher Profile
        [Authorize(Roles = "4")]
        [HttpGet("teacher")]
        public async Task<IActionResult> GetAllTeacherProfiles()
        {
            var teachers = await _teacherService.GetAllTeachersAsync();
            return Ok(teachers);
        }

        [Authorize(Roles = "3,4")]
        [HttpGet("teacher/{userId}")]
        public async Task<IActionResult> GetTeacherProfileByUserId(int userId)
        {
            var teacher = await _teacherService.GetTeacherByUserIdAsync(userId);
            if (teacher == null) return NotFound(new { message = "Teacher profile not found" });
            return Ok(teacher);
        }

        [Authorize(Roles = "4")]
        [HttpPost("teacher")]
        public async Task<IActionResult> CreateTeacherProfile([FromBody] TeacherDtoRequest dto)
        {
            var result = await _teacherService.CreateTeacherAsync(dto);
            if (result > 0)
                return CreatedAtAction(nameof(GetTeacherProfileByUserId), new { userId = dto.UserId }, dto);
            return BadRequest(new { message = "Failed to create teacher profile" });
        }

        [Authorize(Roles = "3,4")]
        [HttpPut("teacher/{id}")]
        public async Task<IActionResult> UpdateTeacherProfile(int id, [FromBody] TeacherDtoRequest dto)
        {
            var result = await _teacherService.UpdateTeacherAsync(dto);
            if (result > 0) return Ok(new { message = "Update successfully" });
            return NotFound(new { message = "Teacher profile not found" });
        }

        [Authorize(Roles = "4")]
        [HttpDelete("teacher/{id}")]
        public async Task<IActionResult> DeleteTeacherProfile(int id)
        {
            var result = await _teacherService.DeleteTeacherAsync(id);
            if (result > 0) return Ok(new { message = "Delete successfully" });
            return NotFound(new { message = "Teacher profile not found" });
        }
        #endregion

        #region Parent Profile
        [Authorize(Roles = "4")]
        [HttpGet("parent")]
        public async Task<IActionResult> GetAllParentProfiles()
        {
            var parents = await _parentService.GetAllParentsAsync();
            return Ok(parents);
        }

        [Authorize(Roles = "2,4")]
        [HttpGet("parent/{userId}")]
        public async Task<IActionResult> GetParentProfileByUserId(int userId)
        {
            var parent = await _parentService.GetParentByUserIdAsync(userId);
            if (parent == null) return NotFound(new { message = "Parent profile not found" });
            return Ok(parent);
        }

        [Authorize(Roles = "4")]
        [HttpPost("parent")]
        public async Task<IActionResult> CreateParentProfile([FromBody] ParentDtoRequest dto)
        {
            var result = await _parentService.CreateParentAsync(dto);
            if (result > 0)
                return CreatedAtAction(nameof(GetParentProfileByUserId), new { userId = dto.UserId }, dto);
            return BadRequest(new { message = "Failed to create parent profile" });
        }

        [Authorize(Roles = "2,4")]
        [HttpPut("parent/{id}")]
        public async Task<IActionResult> UpdateParentProfile(int id, [FromBody] ParentDtoRequest dto)
        {
            var result = await _parentService.UpdateParentAsync(dto);
            if (result > 0) return Ok(new { message = "Update successfully" });
            return NotFound(new { message = "Parent profile not found" });
        }

        [Authorize(Roles = "4")]
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
