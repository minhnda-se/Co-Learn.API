using CoLearn.Domain.Common;
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
    public class SubmissionService : ISubmissionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubmissionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<SubmissionResponseDto>> CreateAsync(int assignmentId, SubmissionRequestDto dto)
        {
            var submission = new Submission
            {
                AssignmentId = assignmentId,
                StudentId = dto.StudentId,
                SubmittedAt = DateTime.UtcNow,
                FilePath = dto.FilePath,
                Grade = null,
                Feedback = null,
                IsDeleted = false
            };

            try
            {
                var created = await _unitOfWork.SubmissionRepository.CreateAsync(submission);
                await _unitOfWork.CommitAsync();
                return Result<SubmissionResponseDto>.Success(MapToResponse(created), "Submission created successfully");
            }
            catch (InvalidOperationException ex)
            {
                return Result<SubmissionResponseDto>.Failure(ex.Message);
            }
        }

        public async Task<Result<List<SubmissionResponseDto>>> GetByAssignmentAsync(int assignmentId)
        {
            var subs = await _unitOfWork.SubmissionRepository.GetByAssignmentIdAsync(assignmentId); if (!subs.Any()) return Result<List<SubmissionResponseDto>>.Failure("No submissions found for this assignment"); var mapped = subs.Select(MapToResponse).ToList(); return Result<List<SubmissionResponseDto>>.Success(mapped);
        }

        public async Task<Result<List<SubmissionResponseDto>>> GetByStudentAsync(int studentId)
        {
            var subs = await _unitOfWork.SubmissionRepository.GetByStudentIdAsync(studentId); 
            if (!subs.Any()) 
                return Result<List<SubmissionResponseDto>>.Failure("No submissions found for this student"); 
            var mapped = subs.Select(MapToResponse).ToList(); 
            return Result<List<SubmissionResponseDto>>.Success(mapped);
        }
        public async Task<Result<SubmissionResponseDto>> GiveFeedbackAsync(long id, SubmissionFeedbackRequestDto dto)
        {
            var submission = await _unitOfWork.SubmissionRepository.GetByIdAsync(id);
            if (submission == null)
                return Result<SubmissionResponseDto>.Failure("Submission not found");

            submission.Grade = dto.Grade;
            submission.Feedback = dto.Feedback;

            await _unitOfWork.SubmissionRepository.UpdateAsync(submission);
            await _unitOfWork.CommitAsync();

            return Result<SubmissionResponseDto>.Success(MapToResponse(submission), "Feedback added successfully");
        }

        private static SubmissionResponseDto MapToResponse(Submission s) { 
            return new SubmissionResponseDto { 
                SubmissionId = s.SubmissionId, 
                SubmittedAt = s.SubmittedAt, 
                Grade = s.Grade, 
                Feedback = s.Feedback, 
                FilePath = s.FilePath, 
                StudentId = s.StudentId,  
                AssignmentId = s.AssignmentId, 
            }; 
        }
    }
}
