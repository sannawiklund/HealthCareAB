using HealthCareABApi.DTO;
using HealthCareABApi.Models;
using HealthCareABApi.Repositories;
using HealthCareABApi.Services;
using Moq;

namespace xUnitTests
{
    public class FeedbackTest
    {
        private readonly Mock<IFeedbackRepository> _feedbackRepositoryMock;
        private readonly FeedbackService _feedbackService;

        public FeedbackTest()
        {
            _feedbackRepositoryMock = new Mock<IFeedbackRepository>();
            _feedbackService = new FeedbackService(_feedbackRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateFeedbackAsync_ValidInput_ShouldCallRepositoryCreateAsync()
        {
            var userId = "user123";
            var feedbackDto = new FeedbackDTO
            {
                AppointmentId = "appointment123",
                Comment = "Great service!"
            };

            await _feedbackService.LeaveFeedbackAsync(userId, feedbackDto);

            _feedbackRepositoryMock.Verify(repo => repo.CreateAsync(It.Is<Feedback>(f =>
                f.AppointmentId == feedbackDto.AppointmentId &&
                f.PatientId == userId &&
                f.Comment == feedbackDto.Comment)), Times.Once);
        }

        [Fact]
        public async Task LeaveFeedbackAsync_InvalidInput_ShouldThrowInvalidOperationException()
        {
            var userId = "user123";
            var emptyCommentDto = new FeedbackDTO
            {
                AppointmentId = "appointment123",
                Comment = ""
            };

            var longCommentDto = new FeedbackDTO
            {
                AppointmentId = "appointment123",
                Comment = new string('a', 901)
            };

            var emptyCommentException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _feedbackService.LeaveFeedbackAsync(userId, emptyCommentDto));
            Assert.Equal("Comment cannot be empty.", emptyCommentException.Message);

            var longCommentException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _feedbackService.LeaveFeedbackAsync(userId, longCommentDto));
            Assert.Equal("Comment cannot exceed 900 characters.", longCommentException.Message);

            _feedbackRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Feedback>()), Times.Never);

        }

    }
}
