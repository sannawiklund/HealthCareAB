using Microsoft.AspNetCore.Mvc;
using HealthCareABApi.DTO;
using HealthCareABApi.Services;
using Microsoft.AspNetCore.Authorization;
using HealthCareABApi.Models;

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

        [Authorize(Roles = Roles.User)]
        [HttpGet("/userpage/{userId}")]
        public async Task<IActionResult> GetUser(string userId)
        {
            /* Låter detta ligga kvar. Tidigare lösning som funkade, men kanske inte var optimal?
             * //Kontrollerar användaren
             var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

             if (string.IsNullOrEmpty(userId))
             {
                 return Unauthorized("User ID is missing a token");
             }*/
            try
            {
                var userDto = await _userPageService.GetUserInformationAsync(userId);

                if (userDto == null)
                {
                    return NotFound("User not found");
                }

                return Ok(userDto);

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occured", Details = ex.Message });
            }
        }

        [Authorize(Roles = Roles.User)]
        [HttpPut("/userpage/{userId}")]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] UserDto userDto)
        {

            try
            {
                var result = await _userPageService.UpdateUserInformationAsync(userId, userDto);

                if (!result)
                {
                    return NotFound("Failed to update user information");
                }

                return Ok("User information has been updated");

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occured", Details = ex.Message });
            }
        }

        [Authorize(Roles = Roles.User)]
        [HttpDelete("/userpage/{userId}")]
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
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occured", Details = ex.Message });
            }
        }
    }
}
