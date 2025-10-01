using CoLearn.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Domain.Interfaces.Repositories
{
    public interface IScheduleRepository
    {
        Task<Schedule> CreateAsync(Schedule schedule);
        Task<List<Schedule>> GetByTeacherIdAsync(int teacherId);
        Task<Schedule?> GetByIdAsync(int id);
        Task<Schedule?> UpdateAsync(Schedule schedule);
        Task<bool> DeleteAsync(int id);
    }
}
