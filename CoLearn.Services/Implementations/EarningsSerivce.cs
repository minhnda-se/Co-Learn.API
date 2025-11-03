using CoLearn.Domain.DTOs;
using CoLearn.Domain.Interfaces;
using CoLearn.Domain.Interfaces.Services;
using CoLearn.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Services.Implementations
{
    public class EarningsSerivce : IEarningService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EarningsSerivce(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<TeacherEarningsDto>> GetAllTeacherEarningsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            return await _unitOfWork.EarningRepository.GetAllTeacherEarningsAsync(startDate, endDate);
        }

        public async Task<TeacherEarningsDto?> GetTeacherEarningsByTeacherIdAsync(int teacherId, DateTime? startDate = null, DateTime? endDate = null)
        {
            return await _unitOfWork.EarningRepository.GetTeacherEarningsByTeacherIdAsync(teacherId, startDate, endDate);
        }

        public async Task<RevenueTrendDto> GetTeacherRevenueTrendsAsync(int? teacherId = null, DateTime? startDate = null, DateTime? endDate = null, string groupBy = "month")
        {
            return await _unitOfWork.EarningRepository.GetTeacherRevenueTrendsAsync(teacherId, startDate, endDate, groupBy);
        }

        public async Task<TopTeachersReportDto> GetTopTeachersAsync(DateTime? startDate = null, DateTime? endDate = null, int limit = 10)
        {
            return await _unitOfWork.EarningRepository.GetTopTeachersAsync(startDate, endDate, limit);
        }
    }
}
