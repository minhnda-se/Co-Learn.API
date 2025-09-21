using AutoMapper;
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

        public async Task<List<AssignmentResponseDto>> GetByLessonIdAsync(int lessonId)
        {
            var assignments = await _unitOfWork.AssignmentRepository.GetByLessonIdAsync(lessonId);
            return _mapper.Map<List<AssignmentResponseDto>>(assignments);
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
    }
}
