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
    public interface ITeacherService
    {
        Task<TeacherDtoResponse?> GetTeacherByIdAsync(int teacherId);
        Task<List<TeacherDtoResponse>> GetAllTeachersAsync();
        Task<int> CreateTeacherAsync(TeacherDtoRequest dto);
        Task<int> UpdateTeacherAsync(TeacherDtoRequest dto);
        Task<int> DeleteTeacherAsync(int teacherId);
    }
}
