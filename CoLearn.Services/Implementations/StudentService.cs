using AutoMapper;
using CoLearn.Domain.Common;
using CoLearn.Domain.DTOs.Requests;
using CoLearn.Domain.DTOs.Responses;
using CoLearn.Domain.Enums;
using CoLearn.Domain.Interfaces;
using CoLearn.Domain.Interfaces.Services;
using CoLearn.Domain.Models;
using CoLearn.Services.Handler;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CoLearn.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;
        private readonly IBackgroundJobService _backgroundJobService;

        public StudentService(IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService, IBackgroundJobService backgroundJobService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _notificationService = notificationService;
            _backgroundJobService = backgroundJobService;
        }

        public async Task<Result<string>> CreateStudentAsync(StudentDtoRequest dto)
        {
            var userExists = await _unitOfWork.UserRepository.GetByEmailAsync(dto.Email);
            if (userExists != null) return Result<string>.Failure("Email already exists");
            // 1 Generate random password
            var password = GenerateRandomPassword(10);
            var token = Guid.NewGuid().ToString("N");
            // 2 Create user
            var user = new User
            {
                FullName = dto.FullName,
                DateOfBirth = dto.Born,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                PrimaryRoleId = (int)UserRoleEnum.Student, 
                IsActive = false,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                VerificationToken = token,
                VerificationTokenExpiry = DateTime.UtcNow.AddMinutes(5),

            };
            await _unitOfWork.UserRepository.CreateAsync(user);
            await _unitOfWork.CommitAsync(); // Để có userId
            // Gửi email xác minh
            var verifyLink = $"https://localhost:7142/api/auth/verify?type={(int)VerifyTypeEnum.StudentCreate}&token={token}";
            string emailBody = $@"<html>
<head>
  <meta charset=""UTF-8"">
  <style>
    body {{
      background-color: #f5f7fa; /* nền nhạt, dịu mắt */
      font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
      margin: 0;
      padding: 0;
      color: #212121; /* chữ chính tối, dễ đọc */
    }}
    .email-wrapper {{
      max-width: 600px;
      margin: 40px auto;
      background-color: #ffffff;
      border-radius: 12px;
      box-shadow: 0 4px 16px rgba(0,0,0,0.08);
      overflow: hidden;
    }}
    .header {{
      background-color: #0d47a1; /* xanh đậm, tương phản cao với chữ trắng */
      color: #ffffff;
      text-align: center;
      padding: 30px 20px;
      font-size: 24px;
      font-weight: bold;
    }}
    .content {{
      padding: 30px 20px;
      line-height: 1.6;
      font-size: 16px;
    }}
    .content p {{
      margin: 16px 0;
    }}
    .btn {{
      display: inline-block;
      padding: 14px 28px;
      font-size: 16px;
      color: #ffffff !important;
      background-color: #1976d2; /* xanh button tương phản cao với nền */
      text-decoration: none;
      border-radius: 8px;
      margin-top: 20px;
      font-weight: 600;
    }}
    .btn:hover {{
      background-color: #0d47a1; /* hover nhấn mạnh */
    }}
    .footer {{
      font-size: 12px;
      color: #666666; /* footer dịu mắt */
      text-align: center;
      padding: 20px;
      border-top: 1px solid #e0e0e0;
    }}
    @media screen and (max-width: 640px) {{
      .email-wrapper {{
        margin: 20px;
      }}
      .header {{
        font-size: 20px;
      }}
      .btn {{
        width: 100%;
        text-align: center;
      }}
    }}
  </style>
</head>
<body>
  <div class=""email-wrapper"">
    <div class=""header"">Verify Your Email</div>
    <div class=""content"">
      <p>Hello {(!string.IsNullOrWhiteSpace(user.FullName) ? user.FullName : "there")},</p>
      <p>Thank you for signing up! Please verify your email by clicking the button below:</p>
        <p>This verification link will expire in 5 minutes.</p>

      <a href=""{verifyLink}"" class=""btn"">🔗 Verify Email</a>
      <p>If you did not register, you can safely ignore this email.</p>
    </div>
    <div class=""footer"">
      &copy; 2025 Co&Learn. All rights reserved.
    </div>
  </div>
</body>
</html>
";
            await _notificationService.SendEmailAsync(user.Email, "Verify your email", emailBody);


            // Schedule job xoá nếu chưa verify sau 5 phút
            _backgroundJobService.Schedule<UserHandler>(
                job => job.DeleteIfNotVerifiedAsync(user.UserId),
                TimeSpan.FromMinutes(5),
                JobType.EmailVerification,
                "User",
                user.UserId
            );
            // 3 Create / update UserProfile
            var profile = new UserProfile
            {
                UserId = user.UserId,
                AvatarUrl = dto.Photo,
                UpdatedAt = DateTime.UtcNow
            };
            _unitOfWork.UserProfileRepository.Add(profile);

            // 4️ Create Student entity linked to user
            var student = new Student
            {
                UserId = user.UserId,
                ParentId = dto.ParentId, // nếu có
                GradeLevel = dto.GradeLevel,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.StudentRepository.AddAndSaveAsync(student);

            await _unitOfWork.CommitAsync();

            // 5️ Send email with login info
            var parent = await _unitOfWork.ParentRepository.GetParentByIdAsync(dto.ParentId);
            emailBody = $@"
            <p>Xin chào <b>{dto.FullName}</b>,</p>
            <p>Tài khoản học sinh của bạn đã được tạo thành công.</p>
            <p><b>Email:</b> {dto.Email}<br>
            <b>Mật khẩu:</b> {password}</p>
            <p>Vui lòng đăng nhập và đổi mật khẩu sau khi truy cập.</p>
            <br>
            <p>Co&Learn Team</p>";
            await _notificationService.SendEmailAsync(parent.User.Email, "Welcome to Co&Learn! 🎉", emailBody);

            return Result<string>.Success("Create successful. Please verify email to active your child account.");
        }

        private string GenerateRandomPassword(int length = 10)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        public async Task<int> UpdateStudentAsync(int userId, StudentDtoRequest dto)
        {
            var existing = await _unitOfWork.StudentRepository.GetByUserIdAsync(userId);
            if (existing == null || existing.IsDeleted) return 0;

            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user != null)
            {
                user.FullName = dto.FullName;
                user.DateOfBirth = dto.Born;
                user.UpdatedAt = DateTime.UtcNow;
                
            }

            // Update UserProfile
            var profile = await _unitOfWork.UserProfileRepository.GetUserProfileAsync(userId);
            if (profile != null)
            {
                profile.AvatarUrl = dto.Photo;
            }

            _mapper.Map(dto, existing);
            existing.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.CommitAsync();
            return await _unitOfWork.StudentRepository.UpdateAndSaveAsync(existing);
        }

        public async Task<int> DeleteStudentAsync(int studentId)
        {
            var student = await _unitOfWork.StudentRepository.GetByIdAsync(studentId);
            if (student == null) return 0;

            student.IsDeleted = true;
            student.DeletedAt = DateTime.UtcNow;

            return await _unitOfWork.StudentRepository.UpdateAndSaveAsync(student);
        }

        public async Task<List<StudentDtoResponse>> GetAllStudentsAsync()
        {
            var students = await _unitOfWork.StudentRepository.GetAllStudentsAsync();
            return _mapper.Map<List<StudentDtoResponse>>(students);
        }

        public async Task<StudentDtoResponse?> GetStudentByIdAsync(int studentId)
        {
            var student = await _unitOfWork.StudentRepository.GetStudentByIdAsync(studentId);
            return _mapper.Map<StudentDtoResponse?>(student);
        }

        public async Task<StudentDtoResponse?> GetStudentByUserIdAsync(int userId)
        {
            var student = await _unitOfWork.StudentRepository.GetByUserIdAsync(userId);
            return _mapper.Map<StudentDtoResponse?>(student);
        }

        public async Task<List<StudentDtoResponse>> GetStudentsByParentIdAsync(int parentId)
        {
            var students = await _unitOfWork.StudentRepository.GetByParentIdAsync(parentId);
            return _mapper.Map<List<StudentDtoResponse>>(students);
        }

    }
}
