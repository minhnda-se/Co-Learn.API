using CoLearn.Domain.Common;
using CoLearn.Domain.DTOs;
using CoLearn.Domain.Interfaces;
using CoLearn.Domain.Interfaces.Services;
using CoLearn.Domain.Models;

namespace CoLearn.Services.Implementations
{
    public class ScheduleService : IScheduleService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ScheduleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // POST /api/schedules (Teacher)
        public async Task<Result<ScheduleResponseDto>> CreateAsync(ScheduleRequestDto dto)
        {
            var schedule = new Schedule
            {
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                MaxStudents = dto.MaxStudents,
                CourseId = dto.CourseId,
                TeacherId = dto.TeacherId,
                IsRecurring = dto.IsRecurring,
                RecurrenceRule = dto.RecurrenceRule,
                MeetingLink = dto.MeetingLink,
                CreatedAt = DateTime.UtcNow,
                CurrentStudents = 0,
                ScheduleStatusId = 1, // default "Active"
                IsDeleted = false
            };

            var created = await _unitOfWork.ScheduleRepository.CreateAsync(schedule);
            await _unitOfWork.CommitAsync();

            return Result<ScheduleResponseDto>.Success(MapToResponse(created), "Schedule created successfully");
        }

        // DELETE /api/schedules/{id} (Teacher/Admin)
        public async Task<Result> DeleteAsync(int id)
        {
            var result = await _unitOfWork.ScheduleRepository.DeleteAsync(id);
            if (!result) return Result.Failure("Schedule not found");

            await _unitOfWork.CommitAsync();
            return Result.Success("Schedule deleted successfully");
        }

        // GET /api/schedules/teacher/{teacherId}
        public async Task<Result<List<ScheduleResponseDto>>> GetByTeacherIdAsync(int teacherId)
        {
            var schedules = await _unitOfWork.ScheduleRepository.GetByTeacherIdAsync(teacherId);
            if (!schedules.Any())
                return Result<List<ScheduleResponseDto>>.Failure("No schedules found");

            var mapped = schedules.Select(MapToResponse).ToList();
            return Result<List<ScheduleResponseDto>>.Success(mapped);
        }

        // PUT /api/schedules/{id} (Teacher)
        public async Task<Result<ScheduleResponseDto?>> UpdateAsync(int id, ScheduleRequestDto dto)
        {
            var schedule = await _unitOfWork.ScheduleRepository.GetByIdAsync(id);
            if (schedule == null) return Result<ScheduleResponseDto>.Failure("Schedule not found");

            // update fields (overwrite từ dto)
            schedule.StartTime = dto.StartTime;
            schedule.EndTime = dto.EndTime;
            schedule.MaxStudents = dto.MaxStudents;
            schedule.CourseId = dto.CourseId;
            schedule.IsRecurring = dto.IsRecurring;
            schedule.RecurrenceRule = dto.RecurrenceRule;
            schedule.MeetingLink = dto.MeetingLink;

            var updated = await _unitOfWork.ScheduleRepository.UpdateAsync(schedule);
            await _unitOfWork.CommitAsync();

            return Result<ScheduleResponseDto>.Success(MapToResponse(updated), "Schedule updated successfully");
        }

        // Mapping entity → DTO
        private static ScheduleResponseDto MapToResponse(Schedule s)
        {
            return new ScheduleResponseDto
            {
                ScheduleId = s.ScheduleId,
                CourseId = s.CourseId ?? 0,
                TeacherId = s.TeacherId,
                StudentId = s.StudentId ?? 0,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                MaxStudents = s.MaxStudents,
                CurrentStudents = s.CurrentStudents,
                Status = s.ScheduleStatus.StatusName,
                IsRecurring = s.IsRecurring,
                RecurrenceRule = s.RecurrenceRule,
                CreatedAt = s.CreatedAt,
                CourseTitle = s.Course?.Title,
                TeacherName = s.Teacher?.User.FullName,
                StudentName = s.Student?.User.FullName,
                MeetingLink = s.MeetingLink
            };
        }

        public async Task<Result> UpdateMeetingLink(int scheduleId, string meetingLink)
        {
            var schedule = await _unitOfWork.ScheduleRepository.GetByIdAsync(scheduleId);
            if (schedule == null)
            {
                return Result<ScheduleResponseDto>.Failure("Không tồn tại lịch!!", 400);
            }
            schedule.MeetingLink = meetingLink;
            await _unitOfWork.CommitAsync();
            return Result.Success("Cập nhật meeting link thành công!");
        }

        public async Task<Result<List<ScheduleResponseDto>>> GetByStudentIdAsync(int stundentId)
        {
            var schedules = await _unitOfWork.ScheduleRepository.GetByStudentIdAsync(stundentId);
            if (!schedules.Any())
                return Result<List<ScheduleResponseDto>>.Failure("No schedules found", 400);

            var mapped = schedules.Select(MapToResponse).ToList();
            return Result<List<ScheduleResponseDto>>.Success(mapped);
        }

        public async Task<Result<ScheduleResponseDto>> GetByIdAsync(int id)
        {
            var schedule = await _unitOfWork.ScheduleRepository.GetByIdAsync(id);
            if (schedule == null)
                return Result<ScheduleResponseDto>.Failure("No schedules found", 400);

            var mapped = MapToResponse(schedule);
            return Result<ScheduleResponseDto>.Success(mapped);
        }
    }
}
