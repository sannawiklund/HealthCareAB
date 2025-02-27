﻿using HealthCareABApi.DTO;
using HealthCareABApi.Models;
using HealthCareABApi.Repositories;
using HealthCareABApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareABApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController : ControllerBase
    {
        private readonly FeedbackService _feedbackService;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly UserService _userService;

        public FeedbackController(
            FeedbackService feedbackService, IAppointmentRepository appointmentRepository, UserService userService)
        {
            _appointmentRepository = appointmentRepository;
            _feedbackService = feedbackService;
            _userService = userService;
        }

        [Authorize(Roles = Roles.User)]
        [HttpPost("/comment/{userId}")]
        public async Task<IActionResult> LeaveFeedback(string userId, [FromBody] FeedbackDTO request)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"User with ID {userId} not found.");
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
