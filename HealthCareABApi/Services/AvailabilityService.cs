using HealthCareABApi.DTO;
using HealthCareABApi.Models;
using HealthCareABApi.Repositories;

namespace HealthCareABApi.Services
{
    public class AvailabilityService
    {
        private readonly IAvailabilityRepository _availabilityRepository;

        public AvailabilityService(IAvailabilityRepository availabilityRepository)
        {
            _availabilityRepository = availabilityRepository; // Repository för tillgänglighet
        }

        public async Task<Availability> CreateAvailabilityAsync(CreateAvailabilityDTO createAvailabilityDTO)
        {
            // Skapa en ny tillgänglighetspost
            var availability = new Availability
            {
                CaregiverId = createAvailabilityDTO.CaregiverId,
                AvailableSlots = createAvailabilityDTO.AvailableSlots
            };

            // Använd servicelagret för att spara tillgängligheten
            await _availabilityRepository.CreateAsync(availability);
            return availability;
            
        }

        public async Task<IEnumerable<AvailabilityDTO>> GetAllAvailabilitiesAsync(string userId)
        {
            var allAvailableSlots = await _availabilityRepository.GetAllAsync();

            var availableSlots = allAvailableSlots
                .SelectMany(a => a.AvailableSlots.Select(appointment => new AvailabilityDTO
                {
                    CaregiverId = a.CaregiverId,
                    AvailableSlots = a.AvailableSlots
                }))
                .ToList();

            return availableSlots;

        }
    }
}