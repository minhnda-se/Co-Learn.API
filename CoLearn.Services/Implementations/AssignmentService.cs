using AutoMapper;
using CoLearn.Domain.Common;
using CoLearn.Domain.DTOs;
using CoLearn.Domain.Interfaces;
using CoLearn.Domain.Interfaces.Services;
using CoLearn.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoLearn.Services.Implementations
{
    public class AssignmentService : IAssignmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AssignmentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<int> CreateAsync(int lessonId, AssignmentRequestDto dto)
        {
            var lesson = await _unitOfWork.AssignmentRepository.GetByIdAsync(lessonId);
            if (lesson == null)
                throw new ArgumentException($"Lesson with ID {lessonId} not found.");

            var assignment = _mapper.Map<Assignment>(dto);
            assignment.LessonId = lessonId;
            assignment.CreatedAt = DateTime.UtcNow;
            assignment.IsDeleted = false;

            await _unitOfWork.AssignmentRepository.AddAndSaveAsync(assignment);

            return assignment.AssignmentId;
        }

        public async Task<int> UpdateAsync(int id, AssignmentRequestDto dto)
        {
            var assignment = await _unitOfWork.AssignmentRepository.GetByIdAsync(id);
            if (assignment == null)
                return 0;

            _mapper.Map(dto, assignment);
            return await _unitOfWork.AssignmentRepository.UpdateAndSaveAsync(assignment);
        }

        public async Task<int> DeleteAsync(int id)
        {
            var assignment = await _unitOfWork.AssignmentRepository.GetByIdAsync(id);
            if (assignment == null)
                return 0;

            assignment.IsDeleted = true;
            assignment.DeletedAt = DateTime.UtcNow;

            return await _unitOfWork.AssignmentRepository.UpdateAndSaveAsync(assignment);
        }

        public async Task<List<SubmissionResponseDto>> GetSubmissionsByAssignmentIdAsync(int assignmentId)
        {
            var submissions = await _unitOfWork.AssignmentRepository.GetSubmissionsByAssignmentIdAsync(assignmentId);
            return _mapper.Map<List<SubmissionResponseDto>>(submissions);
        }

        public async Task<SubmissionResponseDto?> UpdateFeedbackAsync(long submissionId, SubmissionFeedbackRequestDto dto)
        {
            var updated = await _unitOfWork.AssignmentRepository.UpdateFeedbackAsync(submissionId, dto.Grade, dto.Feedback);
            return updated == null ? null : _mapper.Map<SubmissionResponseDto>(updated);
        }

        public async Task<Result<PagedResult<AssignmentResponseDto>>> GetByLessonIdAsync(int pageIndex, int pageSize, int lessonId)
        {
            var pagedAssignments = await _unitOfWork.AssignmentRepository.GetByLessonIdAsync(pageIndex, pageSize, lessonId);
            if (pagedAssignments.Items == null || !pagedAssignments.Items.Any())
                return Result<PagedResult<AssignmentResponseDto>>.Failure("No assignments found for this lesson");

            var mappedItems = pagedAssignments.Items.Select(a => _mapper.Map<AssignmentResponseDto>(a)).ToList();

            var dtoPaged = new PagedResult<AssignmentResponseDto>(
                mappedItems,
                pagedAssignments.PageIndex,
                pagedAssignments.PageSize,
                pagedAssignments.TotalCount
            );
            return Result<PagedResult<AssignmentResponseDto>>.Success(dtoPaged);
        }

        public async Task<Result<AssignmentResponseDto?>> GetByIdAsync(int id)
        {
            var assignment =await _unitOfWork.AssignmentRepository.GetByIdAsync(id);
            if (assignment == null)
                return Result<AssignmentResponseDto?>.Failure("Assignment not found");
            var dto = _mapper.Map<AssignmentResponseDto>(assignment);
            return Result<AssignmentResponseDto?>.Success(dto);
        }
    }
}
