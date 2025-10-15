using CoLearn.Domain.Common;
using CoLearn.Domain.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Domain.Interfaces.Services
{
    public interface IAuthService
    {
        Task<Result<string>> RegisterAsync(UserRequest.CreateUserModel dto);
        Task<Result<string>> VerifyEmailAsync(int type, string token);
        Task<Result> ChangePasswordAsync(int userId, string currentPassword, string newPassword);

    }

}
