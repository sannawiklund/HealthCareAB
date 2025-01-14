using HealthCareABApi.DTO;
using HealthCareABApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareABApi.Services
{
    public class AppointmentService 
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public AppointmentService(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
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
