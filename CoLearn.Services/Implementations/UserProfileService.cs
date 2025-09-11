using CoLearn.Domain.Interfaces;
using CoLearn.Domain.Interfaces.Repositories;
using CoLearn.Domain.Interfaces.Services;
using CoLearn.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Services.Implementations
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserProfileService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> CreateUserProfielAsync(UserProfile userProfile)
        {
            return await _unitOfWork.UserProfileGenericRepository.AddAndSaveAsync(userProfile);
        }

        public async Task<int> DeleteUserProfileAsync(int userId)
        {
            var profile = _unitOfWork.UserProfileGenericRepository.GetById(userId);
            return await _unitOfWork.UserProfileGenericRepository.RemoveAndSaveAsync(profile);
        }

        public async Task<List<UserProfile>> GetAllProfilesAsync()
        {
           return await _unitOfWork.UserProfileRepository.GetAllProfilesAsync();
        }

        public async Task<UserProfile> GetUserProfileAsync(int userId)
        {
            return await _unitOfWork.UserProfileRepository.GetUserProfileAsync(userId);
        }

        public Task<int> UpdateUserProfileAsync(UserProfile userProfile)
        {
            return _unitOfWork.UserProfileGenericRepository.UpdateAndSaveAsync(userProfile);
        }
    }
}
