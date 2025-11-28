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
using static CoLearn.Domain.DTOs.EnrollmentDtos;

namespace CoLearn.Services.Implementations
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EnrollmentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<int> CreateAsync(EnrollmentRequestDto dto)
        {
            var entity = _mapper.Map<Enrollment>(dto);
            entity.EnrolledAt = DateTime.UtcNow;
            entity.IsDeleted = false;

            await _unitOfWork.EnrollmentRepository.AddAndSaveAsync(entity);
            await _unitOfWork.CommitAsync();

            return entity.EnrollmentId;
        }

        public async Task<int> UpdateAsync(int id, EnrollmentRequestDto dto)
        {
            var entity = await _unitOfWork.EnrollmentRepository.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted) return -1;

            _mapper.Map(dto, entity);
            entity.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.EnrollmentRepository.UpdateAndSaveAsync(entity);
            await _unitOfWork.CommitAsync();

            return entity.EnrollmentId;
        }

        public async Task<int> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.EnrollmentRepository.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted) return -1;

            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;

            await _unitOfWork.EnrollmentRepository.UpdateAndSaveAsync(entity);
            await _unitOfWork.CommitAsync();

            return entity.EnrollmentId;
        }

        public async Task<Result<EnrollmentResponseDto?>> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.EnrollmentRepository.GetByIdAsync(id);
            if (entity == null) return Result<EnrollmentResponseDto?>.Failure("Enrollment not found");

            return Result<EnrollmentResponseDto?>.Success(_mapper.Map<EnrollmentResponseDto>(entity));
        }

        public async Task<Result<PagedResult<EnrollmentResponseDto>>> GetAllAsync(int pageIndex, int pageSize)
        {
            var list = await _unitOfWork.EnrollmentRepository.GetAllEnrollmentsAsync(pageIndex, pageSize);
            var dtoList = _mapper.Map<List<EnrollmentResponseDto>>(list.Items);

            return Result<PagedResult<EnrollmentResponseDto>>.Success(new PagedResult<EnrollmentResponseDto>(
                dtoList, pageIndex, pageSize, list.TotalCount));
        }

        public async Task<Result<PagedResult<EnrollmentResponseDto>>> GetByStudentIdAsync(int studentId, int pageIndex, int pageSize)
        {
            var list = await _unitOfWork.EnrollmentRepository.GetByStudentIdAsync(studentId, pageIndex, pageSize);
            var dtoList = _mapper.Map<List<EnrollmentResponseDto>>(list.Items);

            return Result<PagedResult<EnrollmentResponseDto>>.Success(new PagedResult<EnrollmentResponseDto>(
                dtoList, pageIndex, pageSize, list.TotalCount));
        }

        public async Task<Result<PagedResult<EnrollmentResponseDto>>> GetByCourseIdAsync(int courseId, int pageIndex, int pageSize)
        {
            var list = await _unitOfWork.EnrollmentRepository.GetByCourseIdAsync(courseId, pageIndex, pageSize);
            var dtoList = _mapper.Map<List<EnrollmentResponseDto>>(list.Items);

            return Result<PagedResult<EnrollmentResponseDto>>.Success(new PagedResult<EnrollmentResponseDto>(
                dtoList, pageIndex, pageSize, list.TotalCount));
        }
    }
}
