using HealthCareABApi.DTO;
using HealthCareABApi.Models;
using HealthCareABApi.Repositories;
using HealthCareABApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareABApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AvailabilityController : ControllerBase
    {
        private readonly AvailabilityService _availabilityService;
        private readonly AppointmentService _appointmentService;
        private readonly IAppointmentRepository _appointmentRepository;

        private readonly UserService _userService;

        public AvailabilityController(AvailabilityService availabilityService, UserService userService)
        {
            _availabilityService = availabilityService;
            
            _userService = userService;
        }

        // Skapa tillgänglighet för admin
        [Authorize(Roles = Roles.Admin)]
        [HttpPost("/availability/{userId}")]
        public async Task<IActionResult> ScheduleAvailability(string userId, [FromBody] CreateAvailabilityDTO request)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"User with ID {userId} not found.");
            }

            var createAvailability = await _availabilityService.CreateAvailabilityAsync(request);

            // Returnera 201 Created med detaljer om den nya posten
            return CreatedAtAction(
                actionName: nameof(ScheduleAvailability),
                routeValues: new { caregiverId = createAvailability.CaregiverId },
                value: new
                {
                    message = "Availability successfully added",
                    caregiverId = createAvailability.CaregiverId,
                    availableSlots = createAvailability.AvailableSlots
                }
            );
        }

        [Authorize]
        [HttpGet("/availability/{userId}")]
        public async Task<IActionResult> GetAvailableSlots(string userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"User with ID {userId} not found.");
            }

            // Använd servicelagret för att hämta all tillgänglighet
            var allAvailability = await _availabilityService.GetAllAvailabilitiesAsync(userId);

            if (!allAvailability.Any())
            {
                return NotFound("No available slots found");
            }

            return Ok(allAvailability);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPut("cancelAppointment/{appointmentId}/{userId}")]
        public async Task<IActionResult> cancelAppointment(string appointmentId, string userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"User with ID {userId} not found.");
            }
            // Anropa AvailabilityService för att uppdatera status
            var updatedAppointment = await _availabilityService.cancelAppointmentAsync(appointmentId, userId);

            return Ok(new
            {
                message = "Appointment status successfully updated",
                appointmentId = updatedAppointment.Id,
                newStatus = updatedAppointment.Status
            });
        }
    }
}
