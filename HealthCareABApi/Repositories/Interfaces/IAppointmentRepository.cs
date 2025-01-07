using System;
using HealthCareABApi.Models;

namespace HealthCareABApi.Repositories
{
    public interface IAppointmentRepository
    {
        Task<IEnumerable<Appointment>> GetAllAsync();
        Task<Appointment> GetByIdAsync(string id);
        Task CreateAsync(Appointment appointment);
        Task UpdateAsync(string id, Appointment appointment);
        Task DeleteAsync(string id);
    }
}

