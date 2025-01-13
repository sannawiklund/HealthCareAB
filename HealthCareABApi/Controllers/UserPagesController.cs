using Microsoft.AspNetCore.Mvc;
using HealthCareABApi.DTO;
using HealthCareABApi.Repositories;
using System.Security.Claims;

namespace HealthCareABApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserPagesController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserPagesController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            //Kontrollerar användaren
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID is missing a token");
            }

            //Hämtar från databasen VIA repositoryt
            var user = await _userRepository.GetByIdAsync(userId);

            var userDto = new UserDto()
            {
                Username = user.Username,
                Roles = user.Roles
            };

            return Ok(userDto);
        }
    }
}
