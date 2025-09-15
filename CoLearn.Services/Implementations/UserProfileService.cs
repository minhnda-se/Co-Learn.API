using AutoMapper;
using CoLearn.Domain.DTOs;
using CoLearn.Domain.Interfaces;
using CoLearn.Domain.Interfaces.Services;
using CoLearn.Domain.Models;

public class UserProfileService : IUserProfileService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserProfileService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<UserProfile?> GetUserProfileAsync(int userId)
    {
        var profile = await _unitOfWork.UserProfileRepository.GetUserProfileAsync(userId);
        if (profile == null) return null;

        return profile;
    }

    public async Task<List<UserProfile>> GetAllProfilesAsync()
    {
        return await _unitOfWork.UserProfileRepository.GetAllProfilesAsync();
       
    }

    public async Task<int> CreateUserProfileAsync(UserProfileDto dto)
    {
        var existing = await _unitOfWork.UserProfileRepository.GetUserProfileAsync(dto.UserId);

        // Nếu có user có available profile thì không cho tạo
        if (existing != null && existing.IsDeleted == false) return 0;

        // Nếu user có profile nhưng đã bị xóa thì cập nhật lại entity gốc
        if (existing != null && existing.IsDeleted == true)
        {
            _mapper.Map(dto, existing);    // map dữ liệu từ DTO sang entity gốc
            existing.IsDeleted = false;
            existing.UpdatedAt = DateTime.UtcNow;

            return await _unitOfWork.UserProfileRepository.UpdateAndSaveAsync(existing);
        }

        // Nếu chưa có profile nào → tạo mới
        var entity = _mapper.Map<UserProfile>(dto);
        entity.UpdatedAt = DateTime.UtcNow;

        return await _unitOfWork.UserProfileRepository.AddAndSaveAsync(entity);
    }


    public async Task<int> UpdateUserProfileAsync(UserProfileDto dto)
    {
        var profile = await _unitOfWork.UserProfileRepository.GetUserProfileAsync(dto.UserId);
        if (profile == null || profile.IsDeleted == true) return 0;

        _mapper.Map(dto, profile); // map dữ liệu từ DTO sang entity gốc
        profile.UpdatedAt = DateTime.UtcNow;

        return await _unitOfWork.UserProfileRepository.UpdateAndSaveAsync(profile);
    }

    public async Task<int> DeleteUserProfileAsync(int userId)
    {
        var profile = await _unitOfWork.UserProfileRepository.GetUserProfileAsync(userId);
        if (profile == null) return 0;

        profile.IsDeleted = true;
        profile.DeletedAt = DateTime.UtcNow;

        return await _unitOfWork.UserProfileRepository.UpdateAndSaveAsync(profile);
    }
}
