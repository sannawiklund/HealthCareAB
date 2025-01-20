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
        private readonly UserService _userService;

        public AvailabilityController(AvailabilityService availabilityService, UserService userService)
        {
            _availabilityService = availabilityService;
            _userService = userService;
        }

        // Skapa tillgänglighet för admin
        [Authorize]
        [HttpPost("{id}")]
        public async Task<IActionResult> ScheduleAvailability(string id,[FromBody] CreateAvailabilityDTO request)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAvailableSlots(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            // Använd servicelagret för att hämta all tillgänglighet
            var allAvailability = await _availabilityService.GetAllAvailabilitiesAsync(id);

            if (!allAvailability.Any())
            {
                return NotFound("No available slots found");
            }

            return Ok(allAvailability);
        }
    }
}
