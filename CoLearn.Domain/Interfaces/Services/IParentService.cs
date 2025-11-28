using CoLearn.Domain.DTOs.Requests;
using CoLearn.Domain.DTOs.Responses;
using CoLearn.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Domain.Interfaces.Services
{
    public interface IParentService
    {
        Task<ParentDtoResponse?> GetParentByIdAsync(int parentId);
        Task<ParentDtoResponse?> GetParentByUserIdAsync(int userId);
        Task<List<ParentDtoResponse>> GetAllParentsAsync();
        Task<int> CreateParentAsync(ParentDtoRequest dto);
        Task<int> UpdateParentAsync(ParentDtoRequest dto);
        Task<int> DeleteParentAsync(int parentId);
    }
}
