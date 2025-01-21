using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HealthCareABApi.Controllers;
using HealthCareABApi.DTO;
using HealthCareABApi.Models;
using HealthCareABApi.Repositories;
using HealthCareABApi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace HealthCareABApi.Tests
{
    public class AppointmentServiceTests
    {
        private readonly Mock<IAppointmentRepository> _appointmentRepositoryMock;
        private readonly Mock<IAvailabilityRepository> _availabilityRepositoryMock;
        private readonly AppointmentService _appointmentService;

        public AppointmentServiceTests()
        {
            _appointmentRepositoryMock = new Mock<IAppointmentRepository>();
            _availabilityRepositoryMock = new Mock<IAvailabilityRepository>();
            _appointmentService = new AppointmentService(_appointmentRepositoryMock.Object, _availabilityRepositoryMock.Object);
        }

        // POST/Appointment/Book Appointment
        [Fact]
        public async Task BookAppointmentAsync_ShouldBookAppointment_WhenSlotIsAvailable()
        {
            // Arrange
            string userId = "user123";
            var appointmentRequest = new AppointmentDTO
            {
                CaregiverId = "caregiver123",
                AppointmentTime = new DateTime(2025, 1, 22, 14, 0, 0)
            };

            var availability = new Availability
            {
                Id = "availability123",
                CaregiverId = "caregiver123",
                AvailableSlots = new List<DateTime> { new DateTime(2025, 1, 22, 14, 0, 0) }
            };

            _availabilityRepositoryMock
                .Setup(repo => repo.GetByCaregiverIdAsync(appointmentRequest.CaregiverId))
                .ReturnsAsync(new List<Availability> { availability });

            _appointmentRepositoryMock
                .Setup(repo => repo.CreateAsync(It.IsAny<Appointment>()))
                .Returns(Task.CompletedTask);

            _availabilityRepositoryMock
                .Setup(repo => repo.UpdateAsync(availability.Id, It.IsAny<Availability>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _appointmentService.BookAppointmentAsync(userId, appointmentRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.PatientId);
            Assert.Equal(appointmentRequest.CaregiverId, result.CaregiverId);
            Assert.Equal(appointmentRequest.AppointmentTime, result.DateTime);
            Assert.Equal(AppointmentStatus.Scheduled, result.Status);

            _appointmentRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Appointment>()), Times.Once);
            _availabilityRepositoryMock.Verify(repo => repo.UpdateAsync(availability.Id, It.Is<Availability>(a => !a.AvailableSlots.Contains(appointmentRequest.AppointmentTime))), Times.Once);
        }

        [Fact]
        public async Task BookAppointmentAsync_ShouldThrowException_WhenSlotIsNotAvailable()
        {
            // Arrange
            string userId = "user123";
            var appointmentRequest = new AppointmentDTO
            {
                CaregiverId = "caregiver123",
                AppointmentTime = new DateTime(2025, 1, 22, 14, 0, 0)
            };

            _availabilityRepositoryMock
                .Setup(repo => repo.GetByCaregiverIdAsync(appointmentRequest.CaregiverId))
                .ReturnsAsync(new List<Availability>()); // No availability

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _appointmentService.BookAppointmentAsync(userId, appointmentRequest));
            Assert.Equal("The selected slot is not available", exception.Message);

            _appointmentRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Appointment>()), Times.Never);
            _availabilityRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<string>(), It.IsAny<Availability>()), Times.Never);
        }


        //GET/Appointment/Upcoming

        [Fact]
        public async Task GetAppointmentsForUserAsync_ShouldReturnUpcomingAppointments_WhenUserIsLoggedIn()
        {
            // Arrange
            string userId = "user123";
            var now = DateTime.UtcNow;
            var futureAppointment = new Appointment
            {
                PatientId = userId,
                CaregiverId = "caregiver123",
                DateTime = now.AddDays(1),
                Status = AppointmentStatus.Scheduled
            };

            var pastAppointment = new Appointment
            {
                PatientId = userId,
                CaregiverId = "caregiver123",
                DateTime = now.AddDays(-1),
                Status = AppointmentStatus.Completed
            };

            _appointmentRepositoryMock
                .Setup(repo => repo.GetByPatientIdAsync(userId))
                .ReturnsAsync(new List<Appointment> { futureAppointment, pastAppointment });

            // Act
            var result = await _appointmentService.GetAppointmentsForUserAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result); // Endast framtida möten ska returneras
            Assert.Equal(futureAppointment.CaregiverId, result[0].CaregiverId);
            Assert.Equal(futureAppointment.DateTime, result[0].AppointmentTime);
            Assert.Equal(futureAppointment.Status, result[0].Status);

            _appointmentRepositoryMock.Verify(repo => repo.GetByPatientIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetAppointmentsForUserAsync_ThrowsException_WhenRepositoryFails()
        {
            // Arrange
            var userId = "user123";

            _appointmentRepositoryMock
                .Setup(repo => repo.GetByPatientIdAsync(userId))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _appointmentService.GetAppointmentsForUserAsync(userId)
            );

            Assert.Equal("Database error", exception.Message);
        }

        // GET/Appointment/History

        [Fact]
        public async Task GetAppointmentHistoryForUserAsync_ShouldReturnHistoricalAppointments_WhenAppointmentsExist()
        {
            // Arrange
            var userId = "user123";
            var now = DateTime.UtcNow;
            var appointments = new List<Appointment>
        {
            new Appointment
            {
                PatientId = userId,
                CaregiverId = "caregiver1",
                DateTime = now.AddMinutes(-30), // Historical (past) appointment
                Status = AppointmentStatus.Scheduled
            },
            new Appointment
            {
                PatientId = userId,
                CaregiverId = "caregiver2",
                DateTime = now.AddMinutes(-10), // Another past appointment
                Status = AppointmentStatus.Cancelled
            }
        };

            _appointmentRepositoryMock.Setup(r => r.GetByPatientIdAsync(userId)).ReturnsAsync(appointments);

            // Act
            var result = await _appointmentService.GetAppointmentHistoryForUserAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count); // Should return 2 appointments
            Assert.All(result, item => Assert.True(item.AppointmentTime <= now)); // All appointments should be in the past
            Assert.Contains(result, item => item.Status == AppointmentStatus.Completed); // One should be Completed
            Assert.Contains(result, item => item.Status == AppointmentStatus.Cancelled); // One should be Cancelled
        }


        [Fact]
        public async Task GetAppointmentHistoryForUserAsync_ShouldReturnEmptyList_WhenNoHistoricalAppointments()
        {
            // Arrange
            var userId = "user123";
            var now = DateTime.UtcNow;
            var appointments = new List<Appointment>(); // No appointments

            _appointmentRepositoryMock.Setup(r => r.GetByPatientIdAsync(userId)).ReturnsAsync(appointments);

            // Act
            var result = await _appointmentService.GetAppointmentHistoryForUserAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result); // Should return an empty list since there are no historical appointments
        }

    }
}

