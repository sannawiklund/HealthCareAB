using System;
namespace HealthCareABApi.DTO
{
    public class CreateAvailabilityDTO
    {
        public string CaregiverId { get; set; }
        public List<DateTime> AvailableSlots { get; set; }
    }
}

