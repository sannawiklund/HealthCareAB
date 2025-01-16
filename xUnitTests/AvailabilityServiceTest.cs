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

            // Mocka att CreateAsync fungerar som förväntat
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
    }
}
