using Moq;
using HealthCareABApi.Models;
using HealthCareABApi.DTO;
using HealthCareABApi.Repositories;
using HealthCareABApi.Services;

namespace HealthCareABApi.Tests
{
    public class UserPageServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IAppointmentRepository> _appointmentRepositoryMock;
        private readonly UserPageService _userPageService;

        public UserPageServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _appointmentRepositoryMock = new Mock<IAppointmentRepository>();
            _userPageService = 
                new UserPageService(_userRepositoryMock.Object, _appointmentRepositoryMock.Object);
        }

        //GET User Information

        [Fact]
        public async Task GetUserInformationAsync_ShouldReturnUserDto_WhenUserExists()
        {
            // Arrange
            var userId = "123";
            var user = new User
            {
                Id = userId,
                Username = "testuser",
                Roles = new List<string> { "Admin", "User" },
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Phone = "1234567890",
                Address = "123 Main St",
                Gender = "Male",
                DateOfBirth = new DateTime(1990, 1, 1)
            };

            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _userPageService.GetUserInformationAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Username, result.Username);
            Assert.Equal(user.Roles, result.Roles);
            Assert.Equal(user.FirstName, result.FirstName);
            Assert.Equal(user.LastName, result.LastName);
            Assert.Equal(user.Email, result.Email);
            Assert.Equal(user.Phone, result.Phone);
            Assert.Equal(user.Address, result.Address);
            Assert.Equal(user.Gender, result.Gender);
            Assert.Equal(user.DateOfBirth, result.DateOfBirth);

            _userRepositoryMock.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetUserInformationAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "123";

            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _userPageService.GetUserInformationAsync(userId);

            // Assert
            Assert.Null(result);
            _userRepositoryMock.Verify(repo => repo.GetByIdAsync(userId), Times.Once);

        }


        // UPDATE User Information
        [Fact]
        public async Task UpdateUserInformationAsync_ShouldUpdateUser_WhenUserExistsAndValidDataProvided()
        {
            // Arrange
            var userId = "123";
            var existingUser = new User
            {
                Id = userId,
                Username = "oldUsername",
                FirstName = "OldFirstName",
                LastName = "OldLastName",
                Email = "old@example.com",
                Phone = "1234567890",
                Address = "Old Address",
                Gender = "Male"
            };

            var updatedUserDto = new UserDto
            {
                Username = "newUsername",
                FirstName = "NewFirstName",
                LastName = "NewLastName",
                Email = "new@example.com",
                Phone = "0987654321",
                Address = "New Address"
            };

            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId))
                .ReturnsAsync(existingUser);

            _userRepositoryMock.Setup(repo => repo.UpdateAsync(userId, It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _userPageService.UpdateUserInformationAsync(userId, updatedUserDto);

            // Assert
            Assert.True(result);
            _userRepositoryMock.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
            _userRepositoryMock.Verify(repo => repo.UpdateAsync(userId, It.Is<User>(u =>
                u.Username == "newUsername" &&
                u.FirstName == "NewFirstName" &&
                u.LastName == "NewLastName" &&
                u.Email == "new@example.com" &&
                u.Phone == "0987654321" &&
                u.Address == "New Address"
            )), Times.Once);
        }

        [Fact]
        public async Task UpdateUserInformationAsync_ShouldReturnFalse_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "123";
            var updatedUserDto = new UserDto
            {
                Username = "newUsername",
                FirstName = "NewFirstName",
                LastName = "NewLastName",
                Email = "new@example.com",
                Phone = "0987654321",
                Address = "New Address"
            };

            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _userPageService.UpdateUserInformationAsync(userId, updatedUserDto);

            // Assert
            Assert.False(result);
            _userRepositoryMock.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
            _userRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<string>(), It.IsAny<User>()), Times.Never);

        }

        // Delete User

        [Fact]
        public async Task DeleteUserAsync_UserExists_ReturnsTrue()
        {
            // Arrange
            var userId = "123";
            var mockAppointments = new List<Appointment>
            {
                new Appointment { Id = "1", Status = AppointmentStatus.Scheduled },
                new Appointment { Id = "2", Status = AppointmentStatus.Scheduled }
            };

            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId))
                .ReturnsAsync(new User { Id = userId });

            _appointmentRepositoryMock.Setup(repo => repo.GetByPatientIdAsync(userId))
                .ReturnsAsync(mockAppointments);

            // Act
            var result = await _userPageService.DeleteUserAsync(userId);

            // Assert
            Assert.True(result);
            _appointmentRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<string>(), It.IsAny<Appointment>()), Times.Exactly(mockAppointments.Count));
            _userRepositoryMock.Verify(repo => repo.DeleteAsync(userId), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_UserDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var userId = "123";

            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _userPageService.DeleteUserAsync(userId);

            // Assert
            Assert.False(result);
            _appointmentRepositoryMock.Verify(repo => repo.GetByPatientIdAsync(It.IsAny<string>()), Times.Never);
            _userRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task DeleteUserAsync_ExceptionThrown_ReturnsFalse()
        {
            // Arrange
            var userId = "123";

            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await Assert.ThrowsAsync<Exception>(() => _userPageService.DeleteUserAsync(userId));

            // Assert
            Assert.Equal("Database error", result.Message);
            _appointmentRepositoryMock.Verify(repo => repo.GetByPatientIdAsync(It.IsAny<string>()), Times.Never);
            _userRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<string>()), Times.Never);
        }

    }
}
