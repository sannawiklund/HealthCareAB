using Microsoft.AspNetCore.Mvc;
using HealthCareABApi.DTO;
using HealthCareABApi.Services;
using System.Security.Claims;

namespace HealthCareABApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserPagesController : ControllerBase
    {
        private readonly UserPageService _userPageService;

        public UserPagesController(UserPageService userPageService)
        {
            _userPageService = userPageService;
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

            var userDto = await _userPageService.GetUserInformationAsync(userId);

            if (userDto == null)
            {
                return NotFound("User not found");
            }

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
            var result = await _userPageService.UpdateUserInformationAsync(userId, userDto);

            if (!result)
            {
                return BadRequest("Failed to update user information");
            }

            return Ok("User information has been updated");

        }

        [HttpDelete("/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            try
            {
                var result = await _userPageService.DeleteUserAsync(userId);

                if (!result)
                {
                    return NotFound(new { Message = "User not found" });
                }

                //If we want to send back some information about the deleted user we could use return OK instead of this.
                return NoContent();
            }
            catch(Exception ex) 
            {
                return StatusCode(500, new { Message = "An error occured", Details = ex.Message });
            }
        }
    }
}
