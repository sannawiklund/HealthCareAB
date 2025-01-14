using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace HealthCareABApi.Models
{
    public class Caregiver
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        // MongoDB ObjectId stored as a string
        public string Id { get; set; }

        public string Username { get; set; }
        public string PasswordHash { get; set; }
        // List of roles, a User can have one or more roles if needed.
        // Not specifying a role during User creation sets it to User by default
        public List<string> Roles { get; set; }
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
