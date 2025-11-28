using CoLearn.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Domain.Interfaces.Repositories
{
    public interface IScheduleRepository
    {
        Task<Schedule> CreateAsync(Schedule schedule);
        Task<List<Schedule>> GetByTeacherIdAsync(int teacherId);
        Task<List<Schedule>> GetByStudentIdAsync(int studentId);

        Task<Schedule?> GetByIdAsync(int id);
        Task<Schedule?> UpdateAsync(Schedule schedule);
        Task<bool> DeleteAsync(int id);
        Task<Schedule> FindAsync(Expression<Func<Schedule, bool>> predicate);
    }
}
