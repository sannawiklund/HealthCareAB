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
    public class FeedbackController : ControllerBase
    {
        private readonly FeedbackService _feedbackService;
        private readonly IAppointmentRepository _appointmentRepository;
        public FeedbackController(
            FeedbackService feedbackService, IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
            _feedbackService = feedbackService;
        }

        [Authorize]
        [HttpPost("leaveFeedback")]
        public async Task<IActionResult> LeaveFeedback([FromBody] FeedbackDTO request)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(request.Comment))
            {
                return BadRequest(new
                {
                    error = "Comment cannot be empty."
                });
            }

            if (request.Comment.Length > 500)
            {
                return BadRequest(new
                {
                    error = "Comment cannot exceed 900 characters."
                });
            }

            var appointment = await _appointmentRepository.GetByIdAsync(request.AppointmentId);

            if (appointment == null)
            {
                return NotFound(new { error = "Appointment not found." });
            }

            if (appointment.PatientId != userId)
            {
                return BadRequest(new
                {
                    error = "You can only leave feedback for your own appointments."
                });
            }

            var feedback = await _feedbackService.LeaveFeedbackAsync(userId, request);

            return Ok(new
            {
                message = "Feedback successfully submitted.",
            });
        }
    }
}
