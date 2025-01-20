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
        [HttpPost("{userId}")]
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
        [HttpGet("upcoming/{userId}")]
        public async Task<IActionResult> GetUserAppointments(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            var appointmentDtos = await _appointmentService.GetAppointmentsForUserAsync(user.Id);

            if (appointmentDtos == null || !appointmentDtos.Any())
            {
                return NotFound("No appointments found for the user");
            }

            return Ok(appointmentDtos);
        }

        [Authorize]
        [HttpGet("history/{userId}")]
        public async Task<IActionResult> GetUserAppointmentHistory(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            var appointmentHistory = await _appointmentService.GetAppointmentHistoryForUserAsync(user.Id);

            if (appointmentHistory == null || !appointmentHistory.Any())
            {
                return NotFound("No appointment history found for the user");
            }

            return Ok(appointmentHistory);
        }

    }
}
