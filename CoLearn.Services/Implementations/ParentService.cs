using AutoMapper;
using CoLearn.Domain.DTOs.Requests;
using CoLearn.Domain.DTOs.Responses;
using CoLearn.Domain.Interfaces;
using CoLearn.Domain.Interfaces.Services;
using CoLearn.Domain.Models;

namespace CoLearn.Services.Implementations
{
    public class ParentService : IParentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ParentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<int> CreateParentAsync(ParentDtoRequest dto)
        {
            var existing = await _unitOfWork.ParentRepository.GetByUserIdAsync(dto.UserId);

            // Nếu đã tồn tại parent active -> không cho tạo
            if (existing != null && existing.IsDeleted == false) return 0;
            // Lấy User
            var user = await _unitOfWork.UserRepository.GetByIdAsync(dto.UserId);
            if (user == null) return 0;

            // Cập nhật thông tin User
            user.FullName = dto.FullName;
            user.Phone = dto.PhoneNumber;
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

            // Nếu có parent đã bị xóa -> khôi phục lại
            if (existing != null && existing.IsDeleted == true)
            {
                _mapper.Map(dto, existing);
                existing.IsDeleted = false;
                existing.CreatedAt = DateTime.UtcNow;

                return await _unitOfWork.ParentRepository.UpdateAndSaveAsync(existing);
            }

            // Nếu chưa có -> tạo mới
            var entity = _mapper.Map<Parent>(dto);
            entity.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.ParentRepository.AddAndSaveAsync(entity);
            return await _unitOfWork.CommitAsync();
        }

        public async Task<int> UpdateParentAsync(ParentDtoRequest dto)
        {
            var existing = await _unitOfWork.ParentRepository.GetByUserIdAsync(dto.UserId);
            if (existing == null || existing.IsDeleted) return 0;
            // Update User
            var user = await _unitOfWork.UserRepository.GetByIdAsync(dto.UserId);
            if (user != null)
            {
                user.FullName = dto.FullName;
                user.Phone = dto.PhoneNumber;
                user.DateOfBirth = dto.Born;
                user.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.UserRepository.UpdateAsync(user);
            }

            // Update UserProfile
            var profile = await _unitOfWork.UserProfileRepository.GetUserProfileAsync(dto.UserId);
            if (profile != null)
            {
                profile.AvatarUrl = dto.Photo;
            }

            _mapper.Map(dto, existing);
            existing.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.CommitAsync();
            return await _unitOfWork.ParentRepository.UpdateAndSaveAsync(existing);
        }

        public async Task<int> DeleteParentAsync(int parentId)
        {
            var parent = await _unitOfWork.ParentRepository.GetByIdAsync(parentId);
            if (parent == null) return 0;

            parent.IsDeleted = true;
            parent.DeletedAt = DateTime.UtcNow;

            return await _unitOfWork.ParentRepository.UpdateAndSaveAsync(parent);
        }

        public async Task<List<ParentDtoResponse>> GetAllParentsAsync()
        {
            var parents = await _unitOfWork.ParentRepository.GetAllParentsAsync();
            return _mapper.Map<List<ParentDtoResponse>>(parents);
        }

        public async Task<ParentDtoResponse?> GetParentByIdAsync(int parentId)
        {
            var parent = await _unitOfWork.ParentRepository.GetParentByIdAsync(parentId);
            return _mapper.Map<ParentDtoResponse?>(parent);
        }

        public async Task<ParentDtoResponse?> GetParentByUserIdAsync(int userId)
        {
            var parent = await _unitOfWork.ParentRepository.GetByUserIdAsync(userId);
            return parent == null ? null : _mapper.Map<ParentDtoResponse>(parent);
        }
    }
}
