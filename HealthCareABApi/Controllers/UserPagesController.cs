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

        [HttpGet("GetUserInformation")]
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


        [HttpPut("UpdateUserInformation")]
        public async Task<IActionResult> UpdateUser([FromBody] UserDto userDto)
        {
            //Kontrollerar användaren
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID is missing a token");
            }

            //Hämtar användarens information
            var currentUserInformation = await _userRepository.GetByIdAsync(userId);

            if (!string.IsNullOrEmpty(userDto.Username))
            {
                currentUserInformation.Username = userDto.Username;
            }

            if (!string.IsNullOrEmpty(userDto.FirstName))
            {
                currentUserInformation.FirstName = userDto.FirstName;
            }

            if (!string.IsNullOrEmpty(userDto.LastName))
            {
                currentUserInformation.LastName = userDto.LastName;
            }

            if (!string.IsNullOrEmpty(userDto.Email))
            {
                currentUserInformation.Email = userDto.Email;
            }

            if (!string.IsNullOrEmpty(userDto.Phone))
            {
                currentUserInformation.Phone = userDto.Phone;
            }

            if (!string.IsNullOrEmpty(userDto.Address))
            {
                currentUserInformation.Address = userDto.Address;
            }


            await _userRepository.UpdateAsync(userId, currentUserInformation);

            return Ok("User information has been updated");

        }

    }
}
