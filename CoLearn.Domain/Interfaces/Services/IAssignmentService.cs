using CoLearn.Domain.Common;
using CoLearn.Domain.DTOs;
using CoLearn.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Domain.Interfaces.Services
{
    public interface IAssignmentService
    {
        Task<int> CreateAsync(int lessonId, AssignmentRequestDto dto);
        Task<int> UpdateAsync(int id, AssignmentRequestDto dto);
        Task<int> DeleteAsync(int id);
        Task<Result<PagedResult<AssignmentResponseDto>>> GetByLessonIdAsync(int pageIndex, int pageSize, int lessonId);
        Task<Result<AssignmentResponseDto?>> GetByIdAsync(int id);
        Task<List<SubmissionResponseDto>> GetSubmissionsByAssignmentIdAsync(int assignmentId);
        Task<SubmissionResponseDto?> UpdateFeedbackAsync(long submissionId, SubmissionFeedbackRequestDto dto);
    }
}
