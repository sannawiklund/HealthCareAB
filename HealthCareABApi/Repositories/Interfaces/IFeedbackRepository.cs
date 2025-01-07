using System;
using HealthCareABApi.Models;

namespace HealthCareABApi.Repositories
{
    public interface IFeedbackRepository
    {
        Task<IEnumerable<Feedback>> GetAllAsync();
        Task<Feedback> GetByIdAsync(string id);
        Task CreateAsync(Feedback feedback);
        Task UpdateAsync(string id, Feedback feedback);
        Task DeleteAsync(string id);
    }
}

