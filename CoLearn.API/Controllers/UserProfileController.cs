using CoLearn.Domain.Interfaces.Services;

using CoLearn.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CoLearn.API
{
    [Route("api/profile")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;

        public UserProfileController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }
        #region Account Profile
        // GET: api/account
        [HttpGet("account")]
        public async Task<IActionResult> GetAllAccountProfiles()
        {
            var users = await _userProfileService.GetAllProfilesAsync();
            return Ok(users);
        }

        // GET: api/profile/account/{id}
        [HttpGet("account/{id}")]
        public async Task<IActionResult> GetAccountProfileById(int id)
        {
            var user = await _userProfileService.GetUserProfileAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }
        #endregion

        #region Student Profile
        // GET: api/profile/account/student
        #endregion
    }
}
