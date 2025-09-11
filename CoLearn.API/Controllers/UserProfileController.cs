using CoLearn.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CoLearn.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;

        public UserProfileController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        // GET: api/UserProfile
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userProfileService.GetAllProfilesAsync();
            return Ok(users);
        }

        // GET: api/UserProfile/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userProfileService.GetUserProfileAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }
    }
}
