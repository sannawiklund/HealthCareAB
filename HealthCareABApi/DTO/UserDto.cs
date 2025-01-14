﻿namespace HealthCareABApi.DTO
{
    public class UserDto
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }

        public string Address { get; set; }

        public string? Gender { get; set; }

        public DateOnly DateOfBirth { get; set; }
    }
}
