using CoLearn.Domain.Common;
using CoLearn.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Domain.Interfaces.Services
{
    public interface ISubmissionService
    {
        Task<Result<SubmissionResponseDto>> CreateAsync(int assignmentId, SubmissionRequestDto dto);
        Task<Result<List<SubmissionResponseDto>>> GetByAssignmentAsync(int assignmentId);
        Task<Result<List<SubmissionResponseDto>>> GetByStudentAsync(int studentId);
        Task<Result<SubmissionResponseDto>> GiveFeedbackAsync(long id, SubmissionFeedbackRequestDto dto);
    
    }
}
