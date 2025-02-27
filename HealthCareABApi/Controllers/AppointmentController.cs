﻿using HealthCareABApi.DTO;
using HealthCareABApi.Models;
using HealthCareABApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareABApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly AppointmentService _appointmentService;
        private readonly UserService _userService;

        public AppointmentController(UserService userService, AppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
            _userService = userService;

        }

        [Authorize(Roles = Roles.User)]
        [HttpPost("/{userId}")]
        public async Task<IActionResult> BookAppointment(string userId, [FromBody] AppointmentDTO request)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"User with ID {userId} not found.");
            }

            var appointment = await _appointmentService.BookAppointmentAsync(user.Id, request);

            var caregiver = await _userService.GetUserByIdAsync(request.CaregiverId);
            string caregiverName = caregiver.Username;

            //Om allt är ok returneras 200.
            return Ok(new
            {
                caregiverName = caregiverName,
                message = "Appointment successfully booked",
                appointmentId = appointment.Id,
                appointmentTime = appointment.DateTime
            });
        }

        [Authorize]
        [HttpGet("/upcoming/{userId}")]
        public async Task<IActionResult> GetUserAppointments(string userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"User with ID {userId} not found.");
            }

            var appointmentDtos = await _appointmentService.GetAppointmentsForUserAsync(user.Id);

            if (appointmentDtos == null || !appointmentDtos.Any())
            {
                return NoContent();
            }

            return Ok(appointmentDtos);
        }

        [Authorize]
        [HttpGet("/history/{userId}")]
        public async Task<IActionResult> GetUserAppointmentHistory(string userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"User with ID {userId} not found.");
            }

            var appointmentHistory = await _appointmentService.GetAppointmentHistoryForUserAsync(user.Id);

            if (appointmentHistory == null || !appointmentHistory.Any())
            {
                return NoContent();
            }

            return Ok(appointmentHistory);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet("/admin/{caregiverId}")]
        public async Task<IActionResult> GetAdminAppointments(string caregiverId)
        {
            var user = await _userService.GetUserByIdAsync(caregiverId);
            if (user == null)
            {
                return NotFound($"User with ID {caregiverId} not found.");
            }

            var appointmentDtos = await _appointmentService.GetAppointmentsForAdminAsync(caregiverId);

            if (appointmentDtos == null || !appointmentDtos.Any())
            {
                return NoContent();
            }

            return Ok(appointmentDtos);
        }

    }
}
