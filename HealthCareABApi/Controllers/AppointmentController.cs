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
        private readonly IAvailabilityRepository _availabilityRepository;
        private readonly AppointmentService _appointmentService;

        public AppointmentController(IAppointmentRepository appointmentRepository, IAvailabilityRepository availabilityRepository,
            AppointmentService appointmentService)
        {
            _appointmentRepository = appointmentRepository;
            _availabilityRepository = availabilityRepository;
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

            //Kollar om den valda tiden är tillgänglig
            var caregiverAvailability = await _availabilityRepository.GetByCaregiverIdAsync(request.CaregiverId);
            if (!caregiverAvailability.Any(a => a.AvailableSlots.Contains(request.AppointmentTime)))
            {
                return BadRequest("The selected slot is not available");
            }

            //Är den tillgänglig skapas ett nytt möte
            var appointment = new Appointment
            {
                PatientId = userId,
                CaregiverId = request.CaregiverId,
                DateTime = request.AppointmentTime,
                Status = AppointmentStatus.Scheduled
            };

            //sparar ner mötet till databasen
            await _appointmentRepository.CreateAsync(appointment);

            //Plockar bort tiden från vårdpersonalens kalender så det inte kan dubbelbokas.
            var availability = caregiverAvailability.FirstOrDefault(a => a.AvailableSlots.Contains(request.AppointmentTime));
            if (availability != null)
            {
                availability.AvailableSlots.Remove(request.AppointmentTime);
                await _availabilityRepository.UpdateAsync(availability.Id, availability);
            }

            //Om allt är ok returneras 200.
            return Ok(new
            {
                message = "Appointment successfully booked",
                appointmentId = appointment.Id,
                appointmentTime = appointment.DateTime
            });

        }

        [Authorize]
        [HttpGet("patient-appointments")]
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
