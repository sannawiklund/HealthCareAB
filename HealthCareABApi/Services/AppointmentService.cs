using HealthCareABApi.DTO;
using HealthCareABApi.Models;
using HealthCareABApi.Repositories;
using HealthCareABApi.Repositories.Implementations;
using Microsoft.AspNetCore.Mvc;

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
            var appointments = await _appointmentRepository.GetByPatientIdAsync(userId);

            var appointmentDtos = appointments.Select(appointment => new AppointmentDTO
            {
                CaregiverId = appointment.CaregiverId,
                AppointmentTime = appointment.DateTime,
                Status = appointment.Status
            }).ToList();

            return appointmentDtos;
        }
    }
}
