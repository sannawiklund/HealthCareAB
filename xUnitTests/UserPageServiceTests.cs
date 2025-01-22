using System.Threading.Tasks;
using Moq;
using Xunit;
using HealthCareABApi.Models;
using HealthCareABApi.DTO;
using HealthCareABApi.Repositories;
using HealthCareABApi.Services;

namespace HealthCareABApi.Tests
{
    public class UserPageServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly UserPageService _userPageService;

        public UserPageServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _userPageService = new UserPageService(_userRepositoryMock.Object);
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
    }
}
