using CoLearn.Domain.Common;
using CoLearn.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Domain.Interfaces.Services
{
    public interface IScheduleService
    {
        Task<Result<ScheduleResponseDto>> CreateAsync(ScheduleRequestDto dto);
        Task<Result<List<ScheduleResponseDto>>> GetByTeacherIdAsync(int teacherId);
        Task<Result<ScheduleResponseDto?>> UpdateAsync(int id, ScheduleRequestDto request);
        Task<Result> DeleteAsync(int id);
    }
}
