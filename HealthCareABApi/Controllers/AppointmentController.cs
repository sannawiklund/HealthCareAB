using HealthCareABApi.DTO;
using HealthCareABApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HealthCareABApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly AppointmentService _appointmentService;
        private readonly UserService _userService;

        public AppointmentController(UserService userService, AppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
            _userService = userService;

        }

        [Authorize]
        [HttpPost("{id}")]
        public async Task<IActionResult> BookAppointment(string id, [FromBody] AppointmentDTO request)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            var appointment = await _appointmentService.BookAppointmentAsync(user.Id, request);

            //Om allt är ok returneras 200.
            return Ok(new
            {
                message = "Appointment successfully booked",
                appointmentId = appointment.Id,
                appointmentTime = appointment.DateTime
            });
        }

        [Authorize]
        [HttpGet("upcoming")]
        public async Task<IActionResult> GetUserAppointments()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID is missing a token");
            }

            var appointmentDtos = await _appointmentService.GetAppointmentsForUserAsync(userId);

            if (appointmentDtos == null || !appointmentDtos.Any())
            {
                return NotFound("No appointments found for the user");
            }

            return Ok(appointmentDtos);
        }

        [Authorize]
        [HttpGet("history")]
        public async Task<IActionResult> GetUserAppointmentHistory()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID is missing a token");
            }

            var appointmentHistory = await _appointmentService.GetAppointmentHistoryForUserAsync(userId);

            if (appointmentHistory == null || !appointmentHistory.Any())
            {
                return NotFound("No appointment history found for the user");
            }

            return Ok(appointmentHistory);
        }

    }
}
