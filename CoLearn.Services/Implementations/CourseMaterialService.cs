using AutoMapper;
using CoLearn.Domain.Common;
using CoLearn.Domain.DTOs;
using CoLearn.Domain.Interfaces;
using CoLearn.Domain.Interfaces.Repositories;
using CoLearn.Domain.Interfaces.Services;
using CoLearn.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoLearn.Services.Implementations
{
    public class CourseMaterialService : ICourseMaterialService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IS3StorageService _s3StorageService;

        public CourseMaterialService(IUnitOfWork unitOfWork, IMapper mapper, IS3StorageService s3StorageService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _s3StorageService = s3StorageService;
        }

        public async Task<int> CreateAsync(int lessonId, CourseMaterialRequestDto dto)
        {
            var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(lessonId);
            if (lesson == null) throw new KeyNotFoundException($"Lesson {lessonId} not found");

            var material = _mapper.Map<CourseMaterial>(dto);
            material.LessonId = lessonId;
            material.CourseId = lesson.CourseId;
            material.CreatedAt = DateTime.UtcNow;
            material.IsDeleted = false;
            // Nếu FE gửi ImageUrl từ temp/, chuyển sang private/
            if (!string.IsNullOrEmpty(dto.Url))
            {
                try
                {
                    // Lấy fileKey từ URL
                    var tempKey = dto.Url.Replace(_s3StorageService.GetBaseUrl(), "");

                    var newKey = tempKey.Replace("temp/", "private/");
                    material.Url = await _s3StorageService.MoveFileAsync(tempKey, newKey);
                }
                catch (Exception ex)
                {
                    // Log lỗi nhưng không ảnh hưởng đến tạo course
                    Console.WriteLine($"Lỗi move ảnh: {ex.Message}");
                }
            }

            await _unitOfWork.CourseMaterialRepository.AddAndSaveAsync(material);

            return material.MaterialId;
        }

        public async Task<int> UpdateAsync(int id, CourseMaterialRequestDto dto)
        {
            var material = await _unitOfWork.CourseMaterialRepository.GetByIdAsync(id);
            if (material == null) return 0;

            _mapper.Map(dto, material);
            _unitOfWork.CourseMaterialRepository.Update(material);
            return await _unitOfWork.CommitAsync();
        }

        public async Task<int> DeleteAsync(int id)
        {
            var material = await _unitOfWork.CourseMaterialRepository.GetByIdAsync(id);
            if (material == null) return 0;

            material.IsDeleted = true;
            material.DeletedAt = DateTime.UtcNow;

            _unitOfWork.CourseMaterialRepository.Update(material);
            return await _unitOfWork.CommitAsync();
        }

        public async Task<Result<PagedResult<CourseMaterialResponseDto>>> GetByLessonIdAsync(int pageIndex, int pageSize, int lessonId)
        {
            var pagedMaterial = await _unitOfWork.CourseMaterialRepository.GetByLessonIdAsync(pageIndex, pageSize, lessonId);

            if (pagedMaterial.Items == null || !pagedMaterial.Items.Any())
                return Result<PagedResult<CourseMaterialResponseDto>>.Failure("No lessons found for this course");

            var mappedItems = pagedMaterial.Items
                .Select(l => _mapper.Map<CourseMaterialResponseDto>(l))
                .ToList();

            var dtoPaged = new PagedResult<CourseMaterialResponseDto>(
                mappedItems,
                pagedMaterial.PageIndex,
                pagedMaterial.PageSize,
                pagedMaterial.TotalCount
            );

            return Result<PagedResult<CourseMaterialResponseDto>>.Success(dtoPaged);
        }

        public async Task<Result<CourseMaterialResponseDto?>> GetByIdAsync(int id)
        {
            var material = await _unitOfWork.CourseMaterialRepository.GetByIdAsync(id);
            if (material == null)
                return Result<CourseMaterialResponseDto?>.Failure("Material not found");
            var dto = _mapper.Map<CourseMaterialResponseDto>(material);
            return Result<CourseMaterialResponseDto?>.Success(dto);
        }
    }
}
