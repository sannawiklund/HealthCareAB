using System.Threading.Tasks;
using HealthCareABApi.Models;
using HealthCareABApi.Repositories;
using HealthCareABApi.Services;
using Moq;
using Xunit;

public class UserPageServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly UserPageService _userPageService;

    public UserPageServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userPageService = new UserPageService(_userRepositoryMock.Object);
    }

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

        //Comment to trig the CI-pipeline
    }
}
