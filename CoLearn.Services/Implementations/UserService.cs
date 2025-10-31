using CoLearn.Domain.Common;
using CoLearn.Domain.DTOs.Request;
using CoLearn.Domain.DTOs.Response;
using CoLearn.Domain.Interfaces;
using CoLearn.Domain.Interfaces.Repositories;
using CoLearn.Domain.Interfaces.Services;
using CoLearn.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public UserService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<Result<UserReponse.GetUserModel?>> GetByIdAsync(int id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (user == null)
                return Result<UserReponse.GetUserModel>.Failure("User not found");

            return Result<UserReponse.GetUserModel>.Success(MapToResponse(user));
        }

        public async Task<Result<PagedResult<UserReponse.GetUserModel>>> GetAllAsync(int pageIndex, int pageSize)
        {
            var pagedUsers = await _unitOfWork.UserRepository.GetAllAsync(pageIndex, pageSize);
            if (pagedUsers.Items.Count == 0)
                return Result<PagedResult<UserReponse.GetUserModel>>.Failure("No users found");
            var mapped = pagedUsers.Items.Select(MapToResponse).ToList();
            var response = new PagedResult<UserReponse.GetUserModel>(
                mapped, pagedUsers.PageIndex, pagedUsers.PageSize, pagedUsers.TotalCount
            );
            return Result<PagedResult<UserReponse.GetUserModel>>.Success(response);
        }

        public async Task<Result<UserReponse.GetUserModel>> CreateAsync(UserRequest.CreateUserModel dto)
        {
            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Phone = dto.Phone,
                DateOfBirth = dto.DateOfBirth,
                Gender = dto.Gender,
                PrimaryRoleId = dto.PrimaryRoleId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _unitOfWork.UserRepository.CreateAsync(user);
            await _unitOfWork.CommitAsync();

            return Result<UserReponse.GetUserModel>.Success(MapToResponse(created), "User created successfully");
        }

        public async Task<Result<UserReponse.GetUserModel?>> UpdateAsync(int id, UserRequest.UpdateUserModel dto)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (user == null) return Result<UserReponse.GetUserModel>.Failure("User not found");

            user.FullName = dto.FullName ?? user.FullName;
            user.Email = dto.Email ?? user.Email;
            if (!string.IsNullOrEmpty(dto.Password))
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            user.Phone = dto.Phone ?? user.Phone;
            user.DateOfBirth = dto.DateOfBirth ?? user.DateOfBirth;
            user.Gender = dto.Gender ?? user.Gender;
            user.PrimaryRoleId = dto.PrimaryRoleId ?? user.PrimaryRoleId;
            user.IsActive = dto.IsActive ?? user.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            var updated = await _unitOfWork.UserRepository.UpdateAsync(user);
            await _unitOfWork.CommitAsync();

            return Result<UserReponse.GetUserModel>.Success(MapToResponse(updated), "User updated successfully");
        }

        public async Task<Result> DeleteAsync(int id)
        {
            var result = await _unitOfWork.UserRepository.DeleteAsync(id);
            if (!result)
            {
                return Result.Failure("User not found");
            }
            await _unitOfWork.CommitAsync();
            return Result.Success("User deleted successfully");
        }
        public async Task<Result<UserReponse.GetUserModel?>> UnbanAsync(int id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (user == null)
                return Result<UserReponse.GetUserModel?>.Failure("User not found");
            user.IsActive = true;
            user.DeletedAt = null;

            var updated = await _unitOfWork.UserRepository.UpdateAsync(user);
            await _unitOfWork.CommitAsync();

            return Result<UserReponse.GetUserModel?>.Success(MapToResponse(updated), "User has been unbanned");
        }

        public async Task<Result<UserReponse.GetUserModel?>> BanAsync(int id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (user == null)
                return Result<UserReponse.GetUserModel?>.Failure("User not found");
            user.IsActive = false;
            user.DeletedAt = null;

            var updated = await _unitOfWork.UserRepository.UpdateAsync(user);
            await _unitOfWork.CommitAsync();

            return Result<UserReponse.GetUserModel?>.Success(MapToResponse(updated), "User has been unbanned");
        }

        private static UserReponse.GetUserModel MapToResponse(User u)
        {
            return new UserReponse.GetUserModel(
                u.UserId, u.FullName, u.Email, u.Phone, u.DateOfBirth,
                u.Gender, u.PrimaryRoleId, u.IsActive, u.IsDeleted,
                u.DeletedAt, u.CreatedAt, u.UpdatedAt
            );
        }

        // LOGIN
        public async Task<Result<UserReponse.Login?>> LoginAsync(UserRequest.LoginRequest request)
        {
            var user = await _unitOfWork.UserRepository.GetByEmailAsync(request.Email);
            if (user == null) return Result<UserReponse.Login>.Failure("Invalid email");

            if (!user.IsActive) return Result<UserReponse.Login>.Failure("Email is still not verified!");

            // verify password
            bool isValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!isValid) return Result<UserReponse.Login>.Failure("Invalid password");

            // generate token
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);


            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.PrimaryRoleId.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var loginResponse = new UserReponse.Login
            {
                Token = tokenHandler.WriteToken(token),
                Expiration = tokenDescriptor.Expires ?? DateTime.UtcNow.AddHours(2),
                Role = user.PrimaryRoleId.ToString(),
                FullName = user.FullName,
                UserId = user.UserId
            };
            return Result<UserReponse.Login>.Success(loginResponse, "Login successful");
        }
    }
}
