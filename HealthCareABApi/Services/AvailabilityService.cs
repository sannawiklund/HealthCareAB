using HealthCareABApi.Models;
using HealthCareABApi.Repositories;

namespace HealthCareABApi.Services
{
    public class AvailabilityService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IAvailabilityRepository _availabilityRepository;

        public AvailabilityService(IAppointmentRepository appointmentRepository, IAvailabilityRepository availabilityRepository)
        {
            _appointmentRepository = appointmentRepository; // Repository för bokningar
            _availabilityRepository = availabilityRepository; // Repository för tillgänglighet
        }

        public async Task CreateAvailabilityAsync(Availability availability)
        {
            await _availabilityRepository.CreateAsync(availability);
        }

        public async Task<IEnumerable<Availability>> GetAllAvailabilitiesAsync()
        {
            return await _availabilityRepository.GetAllAsync();
        }
    }
}