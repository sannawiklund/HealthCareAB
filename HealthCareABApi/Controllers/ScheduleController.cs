using HealthCareABApi.DTO;
using HealthCareABApi.Models;
using HealthCareABApi.Repositories;
using HealthCareABApi.Repositories.Implementations;
using HealthCareABApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HealthCareABApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScheduleController : ControllerBase
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IAvailabilityRepository _availabilityRepository;

        public ScheduleController(IAppointmentRepository appointmentRepository, IAvailabilityRepository availabilityRepository)
        {
            _appointmentRepository = appointmentRepository; // Repository för bokningar
            _availabilityRepository = availabilityRepository; // Repository för tillgänglighet
        }

        // Skapa tillgänglighet för admin
        [Authorize]
        [HttpPost("scheduleManagement")]
        public async Task<IActionResult> ScheduleManagement([FromBody] CreateAvailabilityDTO request)
        {
            // Hämta admin-ID från token
            var caregiverID = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            // Returnera Unauthorized om admin inte är inloggad
            if (string.IsNullOrEmpty(caregiverID))
            {
                return Unauthorized(new { error = "User is not authorized or token is missing." });
            }

            // Skapa en ny tillgänglighetspost
            var availability = new Availability
            {
                CaregiverId = request.CaregiverId,
                AvailableSlots = request.AvailableSlots
            };

            // Spara tillgänglighet i databasen
            await _availabilityRepository.CreateAsync(availability);

            // Returnera 201 Created med detaljer om den nya posten
            return CreatedAtAction(
                actionName: nameof(ScheduleManagement),
                routeValues: new { caregiverId = availability.CaregiverId },
                value: new
                {
                    message = "Availability successfully added",
                    caregiverId = availability.CaregiverId,
                    availableSlots = availability.AvailableSlots
                }
            );
        }
    }
}
