using AutoMapper;
using CoLearn.Domain.Common;
using CoLearn.Domain.DTOs;
using CoLearn.Domain.DTOs.Requests;
using CoLearn.Domain.DTOs.Responses;
using CoLearn.Domain.Interfaces;
using CoLearn.Domain.Interfaces.Repositories;
using CoLearn.Domain.Interfaces.Services;
using CoLearn.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoLearn.Services.Implementations
{
    public class LessonService : ILessonService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LessonService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<int> CreateAsync(int courseId, LessonRequestDto dto)
        {
            var lesson = _mapper.Map<Lesson>(dto);
            lesson.CourseId = courseId;
            lesson.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.LessonRepository.AddAndSaveAsync(lesson);

            return lesson.LessonId;
        }

        public async Task<int> UpdateAsync(int id, LessonRequestDto dto)
        {
            var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(id);
            if (lesson == null) return 0;

            _mapper.Map(dto, lesson);

            return await _unitOfWork.LessonRepository.UpdateAndSaveAsync(lesson);
        }

        public async Task<int> DeleteAsync(int id)
        {
            var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(id);
            if (lesson == null) return 0;

            lesson.IsDeleted = true;
            lesson.DeletedAt = DateTime.UtcNow;

            return await _unitOfWork.LessonRepository.UpdateAndSaveAsync(lesson);
        }

        public async Task<Result<PagedResult<LessonResponseDto>>> GetByCourseIdAsync(int pageIndex, int pageSize, int courseId)
        {
            var pagedLessons = await _unitOfWork.LessonRepository.GetByCourseIdAsync(pageIndex, pageSize, courseId);

            if (pagedLessons.Items == null || !pagedLessons.Items.Any())
                return Result<PagedResult<LessonResponseDto>>.Failure("No lessons found for this course");

            var mappedItems = pagedLessons.Items
                .Select(l => _mapper.Map<LessonResponseDto>(l))
                .ToList();

            var dtoPaged = new PagedResult<LessonResponseDto>(
                mappedItems,
                pagedLessons.PageIndex,
                pagedLessons.PageSize,
                pagedLessons.TotalCount
            );

            return Result<PagedResult<LessonResponseDto>>.Success(dtoPaged);
        }

        public async Task<Result<LessonResponseDto?>> GetByIdAsync(int id)
        {
            var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(id);

            if (lesson == null)
                return Result<LessonResponseDto?>.Failure("Lesson not found");

            var dto = _mapper.Map<LessonResponseDto>(lesson);
            return Result<LessonResponseDto?>.Success(dto);
        }
    }
}
