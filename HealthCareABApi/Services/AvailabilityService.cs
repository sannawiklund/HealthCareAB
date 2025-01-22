using HealthCareABApi.DTO;
using HealthCareABApi.Models;
using HealthCareABApi.Repositories;
using HealthCareABApi.Repositories.Implementations;

namespace HealthCareABApi.Services
{
    public class AvailabilityService
    {
        private readonly IAvailabilityRepository _availabilityRepository;

        private readonly IAppointmentRepository _appointmentRepository;

        public AvailabilityService(IAvailabilityRepository availabilityRepository,IAppointmentRepository appointmentRepository)
        {
            _availabilityRepository = availabilityRepository; // Repository för tillgänglighet
            _appointmentRepository = appointmentRepository;
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

        public async Task<Appointment> cancelAppointmentAsync(string appointmentId, string userId)
        {
            // Hämta mötet baserat på ID från repository
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);

            if (appointment == null)
            {
                throw new KeyNotFoundException("Appointment not found");
            }

            appointment.Status = AppointmentStatus.Cancelled;
            await _appointmentRepository.UpdateAsync(appointmentId, appointment);

            return appointment;
        }
    }
}
