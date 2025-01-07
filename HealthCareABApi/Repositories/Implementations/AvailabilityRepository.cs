using System;
using HealthCareABApi.Models;
using MongoDB.Driver;

namespace HealthCareABApi.Repositories.Implementations
{
    public class AvailabilityRepository : IAvailabilityRepository
    {
        private readonly IMongoCollection<Availability> _collection;

        public AvailabilityRepository(IMongoDbContext context)
        {
            _collection = context.Availabilities;
        }

        public async Task<IEnumerable<Availability>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<Availability> GetByIdAsync(string id)
        {
            return await _collection.Find(a => a.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Availability availability)
        {
            await _collection.InsertOneAsync(availability);
        }

        public async Task UpdateAsync(string id, Availability availability)
        {
            await _collection.ReplaceOneAsync(a => a.Id == id, availability);
        }

        public async Task DeleteAsync(string id)
        {
            await _collection.DeleteOneAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Availability>> GetByCaregiverIdAsync(string caregiverId)
        {
            return await _collection.Find(a => a.CaregiverId == caregiverId).ToListAsync();
        }

    }
}

