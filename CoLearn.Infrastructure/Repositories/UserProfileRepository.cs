using CoLearn.Domain.Interfaces.Repositories;
using CoLearn.Infrastructure.Context;
using CoLearn.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Infrastructure.Repositories
{
    public class UserProfileRepository : GenericRepository<UserProfile>, IUserProfileRepository
    {
        public UserProfileRepository(AppDbContext context) : base(context) { }

        public async Task<List<UserProfile>> GetAllProfilesAsync()
        {
            return await _context.UserProfiles.Include(p => p.User).ToListAsync();
        }

        public async Task<UserProfile> GetUserProfileAsync(int userId)
        {
            return await _context.UserProfiles.Include(p => p.User)
                                 .FirstOrDefaultAsync(p => p.UserId == userId);
        }
    }
}
