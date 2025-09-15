using CoLearn.Domain.Common;
using CoLearn.Domain.DTOs.Request;
using CoLearn.Domain.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Domain.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserReponse.GetUserModel?> GetByIdAsync(int id);
        Task<PagedResult<UserReponse.GetUserModel>> GetAllAsync(int pageIndex, int pageSize);
        Task<UserReponse.GetUserModel> CreateAsync(UserRequest.CreateUserModel dto);
        Task<UserReponse.GetUserModel?> UpdateAsync(int id, UserRequest.UpdateUserModel dto);
        Task<bool> DeleteAsync(int id);
        Task<UserReponse.Login?> LoginAsync(UserRequest.LoginRequest request);
    }
}
