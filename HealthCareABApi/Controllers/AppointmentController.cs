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

        public AppointmentController(
            AppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }


        //börjar med att kolla så användaren är inloggad, annars ska man inte kunna boka tid.
        [Authorize]
        [HttpPost("bookAppointment")]
        public async Task<IActionResult> BookAppointment([FromBody] AppointmentDTO request)
        {
            //Hämtar användarens token mha claims
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID is missing a token");
            }
            var appointment = await _appointmentService.BookAppointmentAsync(userId, request);

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
