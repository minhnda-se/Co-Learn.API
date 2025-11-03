using CoLearn.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Domain.Interfaces.Repositories
{
    public interface IEarningRepository 
    {
        Task<TeacherEarningsDto?> GetTeacherEarningsByTeacherIdAsync(int teacherId, DateTime? startDate = null, DateTime? endDate = null);

        Task<List<TeacherEarningsDto>> GetAllTeacherEarningsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<TopTeachersReportDto> GetTopTeachersAsync(DateTime? startDate = null, DateTime? endDate = null, int limit = 10);
        Task<RevenueTrendDto> GetTeacherRevenueTrendsAsync(int? teacherId = null, DateTime? startDate = null, DateTime? endDate = null, string groupBy = "month");
    }
}
