using HealthCareABApi.DTO;
using HealthCareABApi.Models;
using HealthCareABApi.Repositories;
using HealthCareABApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HealthCareABApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController : ControllerBase
    {
        private readonly FeedbackService _feedbackService;
        public FeedbackController(
            FeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [Authorize]
        [HttpPost("leaveFeedback")]
        public async Task<IActionResult> LeaveFeedback([FromBody] FeedbackDTO request)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var feedbackId = await _feedbackService.LeaveFeedbackAsync(userId, request);

            return Ok(new
            {
                message = "Feedback successfully submitted.",
                feedbackId = feedbackId
            });
        }
    }
}
