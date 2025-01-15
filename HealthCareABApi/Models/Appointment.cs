using System;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HealthCareABApi.Models
{
    public class Appointment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        // Reference to Patient (User)
        [BsonRepresentation(BsonType.ObjectId)]
        public string PatientId { get; set; }

        // Reference to Caregiver (User)
        [BsonRepresentation(BsonType.ObjectId)]
        public string CaregiverId { get; set; }

        public DateTime DateTime { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AppointmentStatus Status { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AppointmentStatus
    {
        Scheduled,
        Completed,
        Cancelled
    }
}
