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
    public interface IStudentService
    {
        Task<StudentDtoResponse?> GetStudentByIdAsync(int studentId);
        Task<StudentDtoResponse?> GetStudentByUserIdAsync(int userId);
        Task<List<StudentDtoResponse>> GetAllStudentsAsync();
        Task<int> CreateStudentAsync(StudentDtoRequest dto);
        Task<int> UpdateStudentAsync(StudentDtoRequest dto);
        Task<int> DeleteStudentAsync(int studentId);
    }
}
