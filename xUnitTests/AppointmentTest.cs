using HealthCareABApi.DTO;
using HealthCareABApi.Models;
using HealthCareABApi.Repositories;
using HealthCareABApi.Services;
using Moq;

namespace HealthCareABApi.Tests
{
    public class AppointmentServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IAppointmentRepository> _appointmentRepositoryMock;
        private readonly Mock<IAvailabilityRepository> _availabilityRepositoryMock;
        private readonly AppointmentService _appointmentService;

        public AppointmentServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _appointmentRepositoryMock = new Mock<IAppointmentRepository>();
            _availabilityRepositoryMock = new Mock<IAvailabilityRepository>();
            _appointmentService = new AppointmentService(_appointmentRepositoryMock.Object, _availabilityRepositoryMock.Object,
                _userRepositoryMock.Object);
        }

        // POST/Appointment/BookAppointment
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
        public async Task GetAppointmentsForUserAsync_ShouldReturnUpcomingAppointmentsWithCaregiverName_WhenUserIsLoggedIn()
        {
            // Arrange
            string userId = "user123";
            string caregiverId = "caregiver123";
            var now = DateTime.UtcNow;

            var futureAppointment = new Appointment
            {
                PatientId = userId,
                CaregiverId = caregiverId,
                DateTime = now.AddDays(1),
                Status = AppointmentStatus.Scheduled
            };

            var pastAppointment = new Appointment
            {
                PatientId = userId,
                CaregiverId = caregiverId,
                DateTime = now.AddDays(-1),
                Status = AppointmentStatus.Completed
            };

            var caregiver = new User
            {
                Id = caregiverId,
                Username = "CaregiverUser"
            };

            // Mock för _appointmentRepository
            _appointmentRepositoryMock
                .Setup(repo => repo.GetByPatientIdAsync(userId))
                .ReturnsAsync(new List<Appointment> { futureAppointment, pastAppointment });

            // Mock för _userRepository
            _userRepositoryMock
                .Setup(repo => repo.GetByIdAsync(caregiverId))
                .ReturnsAsync(caregiver);

            // Act
            var result = await _appointmentService.GetAppointmentsForUserAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result); // Endast framtida möten ska returneras
            Assert.Equal(futureAppointment.CaregiverId, result[0].CaregiverId);
            Assert.Equal(futureAppointment.DateTime, result[0].AppointmentTime);
            Assert.Equal(futureAppointment.Status, result[0].Status);
            Assert.Equal(caregiver.Username, result[0].CaregiverName); // Kontrollera att CaregiverName är korrekt

            // Verifiera att rätt metoder har kallats
            _appointmentRepositoryMock.Verify(repo => repo.GetByPatientIdAsync(userId), Times.Once);
            _userRepositoryMock.Verify(repo => repo.GetByIdAsync(caregiverId), Times.Once);
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
        public async Task GetAppointmentHistoryForUserAsync_ShouldReturnHistoricalAppointmentsWithCaregiverName_WhenAppointmentsExist()
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
            Status = AppointmentStatus.Completed
            },

            new Appointment

            { 
            PatientId = userId,
            CaregiverId = "caregiver2",
            DateTime = now.AddMinutes(-10), // Another past appointment
            Status = AppointmentStatus.Cancelled
            }

            };

            var caregiver1 = new User { Id = "caregiver1", Username = "Dr. Smith" };
            var caregiver2 = new User { Id = "caregiver2", Username = "Nurse Jane" };

            // Mocka repositories
            _appointmentRepositoryMock
                .Setup(r => r.GetByPatientIdAsync(userId))
                .ReturnsAsync(appointments);

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync("caregiver1"))
                .ReturnsAsync(caregiver1);

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync("caregiver2"))
                .ReturnsAsync(caregiver2);

            // Act
            var result = await _appointmentService.GetAppointmentHistoryForUserAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count); // Should return 2 appointments
            Assert.All(result, item => Assert.True(item.AppointmentTime <= now)); // All appointments should be in the past
            Assert.Contains(result, item => item.Status == AppointmentStatus.Completed); // One should be Completed
            Assert.Contains(result, item => item.Status == AppointmentStatus.Cancelled); // One should be Cancelled
            Assert.Contains(result, item => item.CaregiverName == "Dr. Smith"); // CaregiverName should be correctly set
            Assert.Contains(result, item => item.CaregiverName == "Nurse Jane");

            // Verifiera att repositories anropas korrekt
            _appointmentRepositoryMock.Verify(r => r.GetByPatientIdAsync(userId), Times.Once);
            _userRepositoryMock.Verify(r => r.GetByIdAsync("caregiver1"), Times.Once);
            _userRepositoryMock.Verify(r => r.GetByIdAsync("caregiver2"), Times.Once);
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

        [Fact]
        public async Task GetAppointmentsForAdminAsync_ShouldReturnUpcomingAppointments()
        {
            // Arrange
            string caregiverId = "caregiver-123";
            string patientId = "patient-456";
            var upcomingDate = DateTime.UtcNow.AddDays(1);

            var appointments = new List<Appointment>
        {
            new Appointment { PatientId = patientId, CaregiverId = caregiverId, DateTime = upcomingDate, Status = AppointmentStatus.Scheduled }
        };

            var patient = new User { Id = patientId, Username = "JohnDoe" };

            _appointmentRepositoryMock.Setup(repo => repo.GetByCaregiverIdAsync(caregiverId))
                .ReturnsAsync(appointments);

            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(patientId))
                .ReturnsAsync(patient);

            // Act
            var result = await _appointmentService.GetAppointmentsForAdminAsync(caregiverId);

            // Assert
            Assert.Single(result);
            Assert.Equal(caregiverId, result[0].CaregiverId);
            Assert.Equal("JohnDoe", result[0].PatientName);
            Assert.Equal(upcomingDate, result[0].AppointmentTime);
            Assert.Equal(AppointmentStatus.Scheduled, result[0].Status);
        }

        [Fact]
        public async Task GetAppointmentsForAdminAsync_ThrowsException_WhenRepositoryFails()
        {
            // Arrange
            var caregiverId = "caregiver-123";

            _appointmentRepositoryMock
                .Setup(repo => repo.GetByCaregiverIdAsync(caregiverId))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _appointmentService.GetAppointmentsForAdminAsync(caregiverId)
            );

            Assert.Equal("Database error", exception.Message);
        }

    }
}

