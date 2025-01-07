using System;
using HealthCareABApi.Models;
using MongoDB.Driver;

namespace HealthCareABApi.Repositories.Implementations
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly IMongoCollection<Feedback> _collection;

        public FeedbackRepository(IMongoDbContext context)
        {
            _collection = context.Feedbacks;
        }

        public async Task<IEnumerable<Feedback>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<Feedback> GetByIdAsync(string id)
        {
            return await _collection.Find(f => f.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Feedback feedback)
        {
            await _collection.InsertOneAsync(feedback);
        }

        public async Task UpdateAsync(string id, Feedback feedback)
        {
            await _collection.ReplaceOneAsync(f => f.Id == id, feedback);
        }

        public async Task DeleteAsync(string id)
        {
            await _collection.DeleteOneAsync(f => f.Id == id);
        }
    }
}

