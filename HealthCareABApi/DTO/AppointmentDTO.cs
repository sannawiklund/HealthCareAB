using HealthCareABApi.Models;

namespace HealthCareABApi.DTO
{
    public class AppointmentDTO
    {

        public string CaregiverId { get; set; }
        public string? CaregiverName { get; set; }
        public DateTime AppointmentTime { get; set; }
        public AppointmentStatus Status { get; set; }

    }
}
