using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HealthCareABApi.Models
{
    public class Availability
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        // Reference to Caregiver (User)
        [BsonRepresentation(BsonType.ObjectId)]
        public string CaregiverId { get; set; }

        public List<DateTime> AvailableSlots { get; set; }
    }
}

