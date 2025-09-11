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
        Task<Teacher> GetTeacherByIdAsync(int teacherId);
        Task<List<Teacher>> GetAllTeachersAsync();
        Task<int> CreateTeacherAsync(Teacher teacher);
        Task<int> UpdateTeacherAsync(Teacher teacher);
        Task<int> DeleteTeacherAsync(int teacherId);
    }
}
