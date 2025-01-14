namespace HealthCareABApi.DTO
{
    public class CaregiverDTO
    {
        public List<string> Roles { get; set; }

        public string Profession { get; set; } // Name of specialist area, for example cardiologist

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }

    }
}
