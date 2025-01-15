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
    public class AvailabilityController : ControllerBase
    {
        private readonly AvailabilityService _availabilityService;

        public AvailabilityController(
        AvailabilityService availabilityService)
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

            // Skapa en ny tillgänglighetspost
            var availability = new Availability
            {
                CaregiverId = request.CaregiverId,
                AvailableSlots = request.AvailableSlots
            };

            // Använd servicelagret för att spara tillgängligheten
            await _availabilityService.CreateAvailabilityAsync(availability);

            // Returnera 201 Created med detaljer om den nya posten
            return CreatedAtAction(
                actionName: nameof(ScheduleAvailability),
                routeValues: new { caregiverId = availability.CaregiverId },
                value: new
                {
                    message = "Availability successfully added",
                    caregiverId = availability.CaregiverId,
                    availableSlots = availability.AvailableSlots
                }
            );
        }

        [Authorize]
        [HttpGet("availableslots")]
        public async Task<IActionResult> GetAvailableSlots()
        {
            // Använd servicelagret för att hämta all tillgänglighet
            var allAvailability = await _availabilityService.GetAllAvailabilitiesAsync();

            var availableSlots = allAvailability
                .SelectMany(a => a.AvailableSlots.Select(appointment => new
                {
                    CaregiverId = a.CaregiverId,
                    AvailableSlot = appointment
                }))
                .ToList();

            if (!availableSlots.Any())
            {
                return NotFound("No available slots found");
            }

            return Ok(availableSlots);
        }

    }
}
