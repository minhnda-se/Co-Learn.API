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
        Task<Result<UserReponse.GetUserModel?>> GetByIdAsync(int id);
        Task<Result<PagedResult<UserReponse.GetUserModel>>> GetAllAsync(int pageIndex, int pageSize);
        Task<Result<UserReponse.GetUserModel>> CreateAsync(UserRequest.CreateUserModel dto);
        Task<Result<UserReponse.GetUserModel?>> UpdateAsync(int id, UserRequest.UpdateUserModel dto);
        Task<Result> DeleteAsync(int id);
        Task<Result<UserReponse.GetUserModel?>> UnbanAsync(int id);

        Task<Result<UserReponse.Login?>> LoginAsync(UserRequest.LoginRequest request);
    }
}
