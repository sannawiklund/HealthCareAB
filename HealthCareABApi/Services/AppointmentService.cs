using HealthCareABApi.DTO;
using HealthCareABApi.Models;
using HealthCareABApi.Repositories;

namespace HealthCareABApi.Services
{
    public class AppointmentService 
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IAvailabilityRepository _availabilityRepository;

        public AppointmentService(IAppointmentRepository appointmentRepository, IAvailabilityRepository availabilityRepository)
        {
            _appointmentRepository = appointmentRepository;
            _availabilityRepository = availabilityRepository;
        }

        public async Task<Appointment> BookAppointmentAsync(string userId, AppointmentDTO request)
        {
            var caregiverAvailability = await _availabilityRepository.GetByCaregiverIdAsync(request.CaregiverId);
            var availability = caregiverAvailability.FirstOrDefault(a => a.AvailableSlots.Contains(request.AppointmentTime));

            if (availability == null)
            {
                throw new InvalidOperationException("The selected slot is not available");
            }
            var appointment = new Appointment
            {
                PatientId = userId,
                CaregiverId = request.CaregiverId,
                DateTime = request.AppointmentTime,
                Status = AppointmentStatus.Scheduled
            };

            await _appointmentRepository.CreateAsync(appointment);
            availability.AvailableSlots.Remove(request.AppointmentTime);
            await _availabilityRepository.UpdateAsync(availability.Id, availability);

            return appointment;
        }
        public async Task<List<AppointmentDTO>> GetAppointmentsForUserAsync(string userId)
        {
            // Hämtar alla möten för användaren
            var appointments = await _appointmentRepository.GetByPatientIdAsync(userId);

            // Filtrerar bort möten som har passerat
            var upcomingAppointments = appointments
                .Where(appointment => appointment.DateTime > DateTime.UtcNow)
                .Select(appointment => new AppointmentDTO
                {
                    CaregiverId = appointment.CaregiverId,
                    AppointmentTime = appointment.DateTime,
                    Status = appointment.Status
                })
                .ToList();

            return upcomingAppointments;
        }


        public async Task<List<AppointmentDTO>> GetAppointmentHistoryForUserAsync(string userId)
        {
            var appointments = await _appointmentRepository.GetByPatientIdAsync(userId);

            // Filtrerar möten till endast historiska (innan nuvarande tid)
            var historicalAppointments = appointments
                .Where(a => a.DateTime <= DateTime.UtcNow)
                .Select(appointment =>
                {
                    // Ändra status till Completed om det inte redan är Cancelled
                    var status = appointment.Status == AppointmentStatus.Cancelled
                        ? AppointmentStatus.Cancelled
                        : AppointmentStatus.Completed;

                    return new AppointmentDTO
                    {
                        CaregiverId = appointment.CaregiverId,
                        AppointmentTime = appointment.DateTime,
                        Status = status
                    };
                })
                .ToList();

            return historicalAppointments;
        }

    }
}
