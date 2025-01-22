using HealthCareABApi.DTO;
using HealthCareABApi.Models;
using HealthCareABApi.Repositories;
using HealthCareABApi.Services;
using Moq;

namespace HealthCareABApi.Tests
{
    public class AvailabilityServiceTests
    {
        private readonly Mock<IAvailabilityRepository> _availabilityRepositoryMock;
        private readonly AvailabilityService _availabilityService;

        public AvailabilityServiceTests()
        {
            _availabilityRepositoryMock = new Mock<IAvailabilityRepository>();
            _availabilityService = new AvailabilityService(_availabilityRepositoryMock.Object);
        }


        //Test av metoden CreateAvailabilityAsync
        [Fact]
        public async Task CreateAvailabilityAsync_ShouldCreateAvailability()
        {
            // Arrange
            var createAvailabilityDTO = new CreateAvailabilityDTO
            {
                CaregiverId = "12345",
                AvailableSlots = new List<DateTime>
                {
                    DateTime.Now,
                    DateTime.Now.AddHours(1)
                }
            };

            _availabilityRepositoryMock
                .Setup(repo => repo.CreateAsync(It.IsAny<Availability>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _availabilityService.CreateAvailabilityAsync(createAvailabilityDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createAvailabilityDTO.CaregiverId, result.CaregiverId);
            Assert.Equal(createAvailabilityDTO.AvailableSlots, result.AvailableSlots);

            // Verifiera att CreateAsync kallades en gång med rätt värden
            _availabilityRepositoryMock.Verify(repo => repo.CreateAsync(It.Is<Availability>(a =>
                a.CaregiverId == createAvailabilityDTO.CaregiverId &&
                a.AvailableSlots == createAvailabilityDTO.AvailableSlots
            )), Times.Once);
        }

        [Fact]
        public async Task CreateAvailabilityAsync_ShouldThrowException_WhenRepositoryFails()
        {
            // Arrange
            var createAvailabilityDTO = new CreateAvailabilityDTO
            {
                CaregiverId = "12345",
                AvailableSlots = new List<DateTime>
                { DateTime.Now,
                  DateTime.Now.AddHours(1)
                }
            };

            // Mocka att CreateAsync kastar ett undantag
            _availabilityRepositoryMock
                .Setup(repo => repo.CreateAsync(It.IsAny<Availability>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _availabilityService.CreateAvailabilityAsync(createAvailabilityDTO));

            Assert.Equal("Database error", exception.Message);

            // Verifiera att CreateAsync faktiskt kallades
            _availabilityRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Availability>()), Times.Once);
        }

        [Fact]
        public async Task CreateAvailabilityAsync_ShouldHandleEmptyAvailableSlots()
        {
            // Arrange
            var createAvailabilityDTO = new CreateAvailabilityDTO
            {
                CaregiverId = "12345",
                AvailableSlots = new List<DateTime>() // Tom lista
            };

            // Mocka att CreateAsync fungerar som förväntat
            _availabilityRepositoryMock
                .Setup(repo => repo.CreateAsync(It.IsAny<Availability>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _availabilityService.CreateAvailabilityAsync(createAvailabilityDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createAvailabilityDTO.CaregiverId, result.CaregiverId);
            Assert.Empty(result.AvailableSlots);

            // Verifiera att CreateAsync kallades en gång med rätt värden
            _availabilityRepositoryMock.Verify(repo => repo.CreateAsync(It.Is<Availability>(a =>
                a.CaregiverId == createAvailabilityDTO.CaregiverId &&
                !a.AvailableSlots.Any() // Kontrollera att AvailableSlots är tom
            )), Times.Once);
        }


        //Test av metoden GetAllAvailabilitiesAsync
        [Fact]
        public async Task GetAllAvailabilitiesAsync_ShouldReturnAvailableSlots_WhenDataExists()
        {
            // Arrange
            var userId = "test-user-id";
            var mockData = new List<Availability>
                { new Availability
                  { CaregiverId = "caregiver-1",
                    AvailableSlots = new List<DateTime>
                    { DateTime.Parse("2025-01-22T10:00:00"),
                      DateTime.Parse("2025-01-22T11:00:00")
                    }

                  },
                  new Availability
                  { CaregiverId = "caregiver-2",
                    AvailableSlots = new List<DateTime> { DateTime.Parse("2025-01-23 14:00") }
                  }
            };

            _availabilityRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(mockData);

            // Act
            var result = await _availabilityService.GetAllAvailabilitiesAsync(userId);

            // Assert
            Assert.NotNull(result); // Kontrollera att resultatet inte är null
            Assert.Equal(3, result.Count()); // Kontrollera att tre slots returneras
            Assert.Contains(result, r => r.CaregiverId == "caregiver-1"); // Kontrollera specifik caregiver
        }

        [Fact]
        public async Task GetAllAvailabilitiesAsync_ShouldReturnEmpty_WhenNoDataExists()
        {
            // Arrange
            var userId = "test-user-id";

            _availabilityRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<Availability>()); // Simulera ingen data

            // Act
            var result = await _availabilityService.GetAllAvailabilitiesAsync(userId);

            // Assert
            Assert.NotNull(result); // Resultatet ska inte vara null
            Assert.Empty(result); // Resultatet ska vara tomt
        }

        [Fact]
        public async Task GetAllAvailabilitiesAsync_ShouldThrowException_WhenRepositoryFails()
        {
            // Arrange
            var userId = "test-user-id";

            _availabilityRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ThrowsAsync(new System.Exception("Database error")); // Simulera ett fel

            // Act & Assert
            await Assert.ThrowsAsync<System.Exception>(() => _availabilityService.GetAllAvailabilitiesAsync(userId));
        }
    }
}

