using CoLearn.Domain.DTOs;
using CoLearn.Domain.Models;

public interface IUserProfileService
{
    Task<UserProfile> GetUserProfileAsync(int userId);
    Task<List<UserProfile>> GetAllProfilesAsync();
    Task<int> CreateUserProfileAsync(UserProfileDto userProfileDto);
    Task<int> UpdateUserProfileAsync(UserProfileDto userProfileDto);
    Task<int> DeleteUserProfileAsync(int userId);
}
