using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HealthCareABApi.Models
{
    public class Feedback
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        // Reference to Appointment
        [BsonRepresentation(BsonType.ObjectId)]
        public string AppointmentId { get; set; }

        // Reference to Patient (User)
        [BsonRepresentation(BsonType.ObjectId)]
        public string PatientId { get; set; }

        public string Comment { get; set; }
    }
}

