using CoLearn.Domain.Common;
using CoLearn.Domain.Enums;
using CoLearn.Domain.Interfaces.Services;
using CoLearn.Domain.Interfaces;
using CoLearn.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoLearn.Services.Handler;
using CoLearn.Domain.DTOs.Request;
using Microsoft.Extensions.Configuration;

namespace CoLearn.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly IBackgroundJobService _backgroundJobService;
        private readonly IConfiguration _config;

        public AuthService(
            IUnitOfWork unitOfWork,
            INotificationService notificationService,
            IBackgroundJobService backgroundJobService,
            IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _backgroundJobService = backgroundJobService;
            _config = config;
        }

        public async Task<Result<string>> RegisterAsync(UserRequest.CreateUserModel dto)
        {
            if (dto == null) return Result<string>.Failure("Invalid data", 400);
            if (dto.PrimaryRoleId == 4) return Result<string>.Failure("Can not execute the request", 403);

            var existing = await _unitOfWork.UserRepository.GetByEmailAsync(dto.Email);
            if (existing != null)
                return Result<string>.Failure("Email already exists");

            var token = Guid.NewGuid().ToString("N");

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Phone = dto.Phone,
                DateOfBirth = dto.DateOfBirth,
                Gender = dto.Gender,
                PrimaryRoleId = dto.PrimaryRoleId,
                IsActive = false,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                VerificationToken = token,
                VerificationTokenExpiry = DateTime.UtcNow.AddMinutes(5),
            };
            
            await _unitOfWork.UserRepository.CreateAsync(user);
            await _unitOfWork.CommitAsync();

            // Gửi email xác minh
            var verifyLink = $"{_config["AppSettings:ApiUrl"]}/api/auth/verify?type={(int)VerifyTypeEnum.Register}&token={token}";
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

            return Result<string>.Success("Registration successful. Please verify your email.");
        }

        public async Task<Result<string>> VerifyEmailAsync(int type, string token)
        {
            var user = await _unitOfWork.UserRepository.GetByVerificationTokenAsync(token);
            if (user == null)
                return Result<string>.Failure("Invalid token");

            if (user.VerificationTokenExpiry < DateTime.UtcNow)
                return Result<string>.Failure("Token expired");

            user.IsActive = true;
            user.VerificationToken = null;
            user.VerificationTokenExpiry = null;
            await _unitOfWork.CommitAsync();
            if (type == (int)VerifyTypeEnum.Register)
            {
                // Tạo profile 
                var profile = new UserProfile
                {
                    UserId = user.UserId,
                    AvatarUrl = null,
                    UpdatedAt = DateTime.UtcNow
                };
                await _unitOfWork.UserProfileRepository.AddAndSaveAsync(profile);

                switch (user.PrimaryRoleId)
                {
                    case 1:
                        var student = new Student
                        {
                            UserId = user.UserId,
                            CreatedAt = DateTime.UtcNow,
                            IsDeleted = false
                        };
                        await _unitOfWork.StudentRepository.AddAndSaveAsync(student);
                        break;

                    case 2:
                        var parent = new Parent
                        {
                            UserId = user.UserId,
                            CreatedAt = DateTime.UtcNow,
                            IsDeleted = false
                        };
                        await _unitOfWork.ParentRepository.AddAndSaveAsync(parent);
                        break;
                    case 3:
                        var teacher = new Teacher
                        {
                            UserId = user.UserId,
                            CreatedAt = DateTime.UtcNow,
                            IsDeleted = false
                        };
                        await _unitOfWork.TeacherRepository.AddAndSaveAsync(teacher);
                        break;
                }
            }
            try
            {
                var deletedCount = await _backgroundJobService.DeleteByTargetAsync("User", user.UserId);
                Console.WriteLine($"Deleted {deletedCount} related Hangfire jobs for UserId={user.UserId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to delete related jobs: {ex.Message}");
            }

            return Result<string>.Success("Email verified successfully");
        }

        public async Task<Result> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null)
                return Result.Failure("User not found.");

            bool passwordValid = BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash);
            if (!passwordValid)
                return Result.Failure("Current password is incorrect.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;
    
            await _unitOfWork.CommitAsync();

            return Result.Success("Password changed successfully.");
        }
    }

}
