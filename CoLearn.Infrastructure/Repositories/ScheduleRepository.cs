using CoLearn.Domain.Interfaces.Repositories;
using CoLearn.Domain.Models;
using CoLearn.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Infrastructure.Repositories
{
    public class ScheduleRepository : GenericRepository<Schedule>, IScheduleRepository
    {
        private readonly AppDbContext _context;

        public ScheduleRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Schedule> CreateAsync(Schedule schedule)
        {
            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();
            return schedule;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null)
                return false;

            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Schedule?> GetByIdAsync(int id)
        {
            return await _context.Schedules
                 .Include(s => s.Course)
        .Include(s => s.Teacher)
            .ThenInclude(t => t.User)
        .Include(s => s.ScheduleStatus)
        .Include(s => s.Student).ThenInclude(st => st.User)
                .FirstOrDefaultAsync(s => s.ScheduleId == id);
        }

        public async Task<List<Schedule>> GetByTeacherIdAsync(int teacherId)
        {
            return await _context.Schedules
                .Where(s => s.TeacherId == teacherId && !s.IsDeleted)
                 .Include(s => s.Course)
        .Include(s => s.Teacher)
            .ThenInclude(t => t.User)
        .Include(s => s.ScheduleStatus)
        .Include(s => s.Student).ThenInclude(st => st.User)
                .ToListAsync();
        }

        public async Task<Schedule?> UpdateAsync(Schedule schedule)
        {
            var existingSchedule = await _context.Schedules.FindAsync(schedule.ScheduleId);
            if (existingSchedule == null)
                return null;

            _context.Entry(existingSchedule).CurrentValues.SetValues(schedule);
            await _context.SaveChangesAsync();
            return existingSchedule;
        }
        public async Task<Schedule> FindAsync(Expression<Func<Schedule, bool>> predicate)
        {
            return await _context.Schedules.FirstOrDefaultAsync(predicate);
        }

        public async Task<List<Schedule>> GetByStudentIdAsync(int studentId)
        {
            return await _context.Schedules
               .Where(s => s.StudentId == studentId && !s.IsDeleted)
               .Include(s => s.Course)
               .Include(s => s.Teacher).ThenInclude(t => t.User)
               .Include(s => s.Student).ThenInclude(st => st.User)
               .Include(s => s.ScheduleStatus)
               .ToListAsync();
        }
    }
}
