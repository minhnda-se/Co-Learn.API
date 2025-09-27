using AutoMapper;
using CoLearn.Domain.DTOs.Requests;
using CoLearn.Domain.DTOs.Responses;
using CoLearn.Domain.Interfaces.Services;
using CoLearn.Domain.Interfaces;
using CoLearn.Domain.Models;

public class TeacherService : ITeacherService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TeacherService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<int> CreateTeacherAsync(TeacherDtoRequest dto)
    {
        var existing = await _unitOfWork.TeacherRepository.GetByUserIdAsync(dto.UserId);

        if (existing != null && !existing.IsDeleted)
            return 0; // User đã có teacher profile

        if (existing != null && existing.IsDeleted)
        {
            _mapper.Map(dto, existing);
            existing.IsDeleted = false;
            existing.CreatedAt = DateTime.UtcNow;

            return await _unitOfWork.TeacherRepository.UpdateAndSaveAsync(existing);
        }

        var entity = _mapper.Map<Teacher>(dto);
        entity.CreatedAt = DateTime.UtcNow;

        return await _unitOfWork.TeacherRepository.AddAndSaveAsync(entity);
    }

    public async Task<int> UpdateTeacherAsync(TeacherDtoRequest dto)
    {
        var existing = await _unitOfWork.TeacherRepository.GetByUserIdAsync(dto.UserId);
        if (existing == null || existing.IsDeleted) return 0;

        _mapper.Map(dto, existing);
        existing.CreatedAt = DateTime.UtcNow;

        return await _unitOfWork.TeacherRepository.UpdateAndSaveAsync(existing);
    }

    public async Task<int> DeleteTeacherAsync(int teacherId)
    {
        var profile = await _unitOfWork.TeacherRepository.GetByUserIdAsync(teacherId);
        if (profile == null) return 0;

        profile.IsDeleted = true;
        profile.DeletedAt = DateTime.UtcNow;

        return await _unitOfWork.TeacherRepository.UpdateAndSaveAsync(profile);
    }

    public async Task<List<TeacherDtoResponse>> GetAllTeachersAsync()
    {
        var teachers = await _unitOfWork.TeacherRepository.GetAllTeachersAsync();
        return _mapper.Map<List<TeacherDtoResponse>>(teachers);
    }

    public async Task<TeacherDtoResponse?> GetTeacherByIdAsync(int teacherId)
    {
        var teacher = await _unitOfWork.TeacherRepository.GetTeacherByIdAsync(teacherId);
        return teacher == null ? null : _mapper.Map<TeacherDtoResponse>(teacher);
    }

    public async Task<TeacherDtoResponse?> GetTeacherByUserIdAsync(int userId)
    {
        var teacher = await _unitOfWork.TeacherRepository.GetByUserIdAsync(userId);
        return teacher == null ? null : _mapper.Map<TeacherDtoResponse>(teacher);
    }
}
