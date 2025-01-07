using System;
using HealthCareABApi.Models;
using MongoDB.Driver;

namespace HealthCareABApi.Repositories.Implementations
{
    public class MongoDbContext : IMongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IConfiguration configuration)
        {
            // Access the MongoDB connection string and database name
            var connectionString = configuration["MongoDbSettings:ConnectionString"];
            var databaseName = configuration["MongoDbSettings:DatabaseName"];

            // Log the values for debugging (optional)
            Console.WriteLine($"Connection String: {connectionString}");
            Console.WriteLine($"Database Name: {databaseName}");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("MongoDB connection string is not configured.");
            }

            if (string.IsNullOrEmpty(databaseName))
            {
                throw new InvalidOperationException("MongoDB database name is not configured.");
            }

            // Initialize MongoDB client and database
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<Appointment> Appointments => _database.GetCollection<Appointment>("Appointments");
        public IMongoCollection<Availability> Availabilities => _database.GetCollection<Availability>("Availabilities");
        public IMongoCollection<Feedback> Feedbacks => _database.GetCollection<Feedback>("Feedbacks");
        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
    }

}

