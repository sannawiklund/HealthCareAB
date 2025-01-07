using System;
using HealthCareABApi.Models;
using MongoDB.Driver;

namespace HealthCareABApi.Repositories.Implementations
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly IMongoCollection<Appointment> _collection;

        public AppointmentRepository(IMongoDbContext context)
        {
            _collection = context.Appointments;
        }

        public async Task<IEnumerable<Appointment>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<Appointment> GetByIdAsync(string id)
        {
            return await _collection.Find(a => a.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Appointment appointment)
        {
            await _collection.InsertOneAsync(appointment);
        }

        public async Task UpdateAsync(string id, Appointment appointment)
        {
            await _collection.ReplaceOneAsync(a => a.Id == id, appointment);
        }

        public async Task DeleteAsync(string id)
        {
            await _collection.DeleteOneAsync(a => a.Id == id);
        }
    }
}

