using CoLearn.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Domain.Interfaces.Repositories
{
    public interface IUserProfileRepository
    {
        Task<UserProfile> GetUserProfileAsync(int userId);
        Task<List<UserProfile>> GetAllProfilesAsync();
        //Task<int> CreateUserProfielAsync(UserProfile userProfile);
        //Task<int> UpdateUserProfileAsync(UserProfile userProfile);
        //Task<int> DeleteUserProfileAsync(int userId);


    }
}
