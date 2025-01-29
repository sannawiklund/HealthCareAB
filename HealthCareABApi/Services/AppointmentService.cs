using HealthCareABApi.DTO;
using HealthCareABApi.Models;
using HealthCareABApi.Repositories;
using HealthCareABApi.Repositories.Implementations;

namespace HealthCareABApi.Services
{
    public class AppointmentService 
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IAvailabilityRepository _availabilityRepository;
        private readonly IUserRepository _userRepository;

        public AppointmentService(IAppointmentRepository appointmentRepository, IAvailabilityRepository availabilityRepository, IUserRepository userRepository)
        {
            _appointmentRepository = appointmentRepository;
            _availabilityRepository = availabilityRepository;
            _userRepository = userRepository;
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
            var appointments = await _appointmentRepository.GetByPatientIdAsync(userId);

            var upcomingAppointments = new List<AppointmentDTO>();

            foreach (var appointment in appointments.Where(a => a.DateTime > DateTime.UtcNow))
            {
                var caregiverId = appointment.CaregiverId;

                var caregiver = await _userRepository.GetByIdAsync(caregiverId);

                upcomingAppointments.Add(new AppointmentDTO
                {
                    CaregiverId = caregiverId,
                    CaregiverName = $"{caregiver.Username}",
                    AppointmentTime = appointment.DateTime,
                    Status = appointment.Status
                });
            }

            return upcomingAppointments;
        }


        public async Task<List<AppointmentDTO>> GetAppointmentHistoryForUserAsync(string userId)
        {
            var appointments = await _appointmentRepository.GetByPatientIdAsync(userId);

            var historicalAppointments = new List<AppointmentDTO>();

            foreach (var appointment in appointments.Where(a => a.DateTime <= DateTime.UtcNow))
            {
                var caregiverId = appointment.CaregiverId;

                var caregiver = await _userRepository.GetByIdAsync(caregiverId);

                var status = appointment.Status == AppointmentStatus.Cancelled
                    ? AppointmentStatus.Cancelled
                    : AppointmentStatus.Completed;

                historicalAppointments.Add(new AppointmentDTO
                {
                    CaregiverId = caregiverId,
                    CaregiverName = $"{caregiver.Username}",
                    AppointmentTime = appointment.DateTime,
                    Status = status
                });
            }

            return historicalAppointments;
        }

        public async Task<List<AppointmentDTO>> GetAppointmentsForAdminAsync(string caregiverId)
        {
            var appointments = await _appointmentRepository.GetByCaregiverIdAsync(caregiverId);

            var upcomingAppointments = new List<AppointmentDTO>();

            foreach (var appointment in appointments.Where(a => a.DateTime > DateTime.UtcNow))
            {
                var userId = appointment.PatientId;

                var user = await _userRepository.GetByIdAsync(userId);

                upcomingAppointments.Add(new AppointmentDTO
                {
                    CaregiverId = caregiverId,
                    PatientName = $"{user.Username}",
                    AppointmentTime = appointment.DateTime,
                    Status = appointment.Status
                });
            }

            return upcomingAppointments;
        }


    }
}
