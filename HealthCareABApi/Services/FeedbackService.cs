using HealthCareABApi.Controllers;
using HealthCareABApi.DTO;
using HealthCareABApi.Models;
using HealthCareABApi.Repositories;

namespace HealthCareABApi.Services
{
    public class FeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;

        public FeedbackService(IFeedbackRepository feedbackRepository)
        {
            _feedbackRepository = feedbackRepository;
        }
        public async Task<string> LeaveFeedbackAsync(string userId, FeedbackDTO feedbackDto)
        {
            if (string.IsNullOrWhiteSpace(feedbackDto.Comment))
            {
                throw new InvalidOperationException("Comment cannot be empty.");
            }

            if (feedbackDto.Comment.Length > 900)
            {
                throw new InvalidOperationException("Comment cannot exceed 900 characters.");
            }

            var feedback = new Feedback
            {
                AppointmentId = feedbackDto.AppointmentId,
                PatientId = userId,
                Comment = feedbackDto.Comment
            };

            await _feedbackRepository.CreateAsync(feedback);

            return feedback.Id;
        }

        
    }
}
