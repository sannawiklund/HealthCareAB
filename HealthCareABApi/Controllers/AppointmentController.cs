using HealthCareABApi.DTO;
using HealthCareABApi.Models;
using HealthCareABApi.Repositories;
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
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly AppointmentService _appointmentService;

        public AppointmentController(IAppointmentRepository appointmentRepository, IAvailabilityRepository availabilityRepository,
            AppointmentService appointmentService)
        {
            _appointmentRepository = appointmentRepository;
            _appointmentService = appointmentService;
        }


        //börjar med att kolla så användaren är inloggad, annars ska man inte kunna boka tid.
        [Authorize]
        [HttpPost]
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
        [HttpGet("Appointments/Upcoming")]
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
    }
}
