using HealthCareABApi.DTO;
using HealthCareABApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HealthCareABApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AvailabilityController : ControllerBase
    {
        private readonly AvailabilityService _availabilityService;

        public AvailabilityController(AvailabilityService availabilityService)
        {
            _availabilityService = availabilityService;
        }

        // Skapa tillgänglighet för admin
        [Authorize]
        [HttpPost("scheduleAvailability")]
        public async Task<IActionResult> ScheduleAvailability([FromBody] CreateAvailabilityDTO request)
        {
            // Hämta admin-ID från token
            var caregiverID = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

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
        [HttpGet("availableslots")]
        public async Task<IActionResult> GetAvailableSlots()
        {
            // Använd servicelagret för att hämta all tillgänglighet
            var allAvailability = await _availabilityService.GetAllAvailabilitiesAsync();

            if (!allAvailability.Any())
            {
                return NotFound("No available slots found");
            }

            return Ok(allAvailability);
        }
    }
}
