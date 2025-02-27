﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HealthCareABApi.Models
{
    public class User
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


        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }

        public string Address { get; set; }

        public string? Gender { get; set; }

        public DateTime DateOfBirth { get; set; }

    }
}
