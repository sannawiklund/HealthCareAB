namespace HealthCareABApi.DTO
{
    public class CaregiverDTO
    {
        public string Profession { get; set; } // Name of specialist area, for example cardiologist


        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }

        public string Address { get; set; }

        public string? Gender { get; set; }

        public DateOnly DateOfBirth { get; set; }
    }
}
