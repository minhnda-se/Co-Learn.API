using AutoMapper;
using CoLearn.Domain.DTOs;
using CoLearn.Domain.DTOs.Requests;
using CoLearn.Domain.DTOs.Responses;
using CoLearn.Domain.Interfaces;
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

        public async Task<List<LessonResponseDto>> GetByCourseIdAsync(int courseId)
        {
            var lessons = await _unitOfWork.LessonRepository.GetByCourseIdAsync(courseId);
            return _mapper.Map<List<LessonResponseDto>>(lessons);
        }
    }
}
