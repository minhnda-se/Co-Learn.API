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
        // Lấy User
        var user = await _unitOfWork.UserRepository.GetByIdAsync(dto.UserId);
        if (user == null) return 0;

        // Cập nhật thông tin User
        user.FullName = dto.FullName;
        user.Phone = dto.Phone;
        user.Gender = dto.Gender;
        user.DateOfBirth = dto.Born;
        user.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.UserRepository.UpdateAsync(user);

        // Cập nhật UserProfile
        var profile = await _unitOfWork.UserProfileRepository.GetUserProfileAsync(dto.UserId);
        if (profile == null)
        {
            profile = new UserProfile
            {
                UserId = dto.UserId,
                AvatarUrl = dto.Photo

            };
             _unitOfWork.UserProfileRepository.Add(profile);
        }
        else
        {
            profile.AvatarUrl = dto.Photo;
            _unitOfWork.UserProfileRepository.Update(profile);
        }

        // Kiểm tra Teacher đã tồn tại chưa
        var teacher = await _unitOfWork.TeacherRepository.GetByUserIdAsync(dto.UserId);

        if (teacher != null && !teacher.IsDeleted)
        {
            return 0; // Đã có teacher profile
        }

        if (teacher != null && teacher.IsDeleted)
        {
            teacher.Qualification = $"{dto.Degree}|{dto.Cv}";
            teacher.Bio = dto.Description;
            teacher.IsDeleted = false;
            teacher.CreatedAt = DateTime.UtcNow;

            _unitOfWork.TeacherRepository.Update(teacher);
        }
        else
        {
            teacher = new Teacher
            {
                UserId = dto.UserId,
                Qualification = $"{dto.Degree}|{dto.Cv}",
                Bio = dto.Description,
                CreatedAt = DateTime.UtcNow
            };
            _unitOfWork.TeacherRepository.Add(teacher);
        }

        return await _unitOfWork.CommitAsync();
    }

    public async Task<int> UpdateTeacherAsync(TeacherDtoRequest dto)
    {
        var teacher = await _unitOfWork.TeacherRepository.GetByUserIdAsync(dto.UserId);
        if (teacher == null || teacher.IsDeleted) return 0;

        // Update User
        var user = await _unitOfWork.UserRepository.GetByIdAsync(dto.UserId);
        if (user != null)
        {
            user.FullName = dto.FullName;
            user.Phone = dto.Phone;
            user.DateOfBirth = dto.Born;
            user.Gender = dto.Gender;
            user.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.UserRepository.UpdateAsync(user);
        }

        // Update UserProfile
        var profile = await _unitOfWork.UserProfileRepository.GetUserProfileAsync(dto.UserId);
        if (profile != null)
        {
            profile.AvatarUrl = dto.Photo;
        }

        // Update Teacher
        teacher.Qualification = $"{dto.Degree}|{dto.Cv}";
        teacher.Bio = dto.Description;

        return await _unitOfWork.CommitAsync();
    }

    public async Task<int> DeleteTeacherAsync(int teacherId)
    {
        var profile = await _unitOfWork.TeacherRepository.GetTeacherByIdAsync(teacherId);
        var user = await _unitOfWork.UserRepository.GetByIdAsync(profile.UserId);
        if (profile == null || user == null) return 0;
        user.IsDeleted = true;
        user.DeletedAt = DateTime.UtcNow;
        profile.IsDeleted = true;
        profile.DeletedAt = DateTime.UtcNow;

        await _unitOfWork.UserRepository.UpdateAsync(user);
        _unitOfWork.TeacherRepository.Update(profile);

        return await _unitOfWork.CommitAsync();
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
