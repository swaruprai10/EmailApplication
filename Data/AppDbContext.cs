using EmailCampaignApp.Models;
using MongoDB.Driver;


namespace EmailCampaignApp.Data
{
    public class AppDbContext
    {
        private readonly IMongoDatabase _database;

        // Constructor to initialize the MongoDB database context
        public AppDbContext(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        // Define collections for each model
        public IMongoCollection<Contact> Contacts => _database.GetCollection<Contact>("Contacts");
        public IMongoCollection<SentEmail> SentEmails => _database.GetCollection<SentEmail>("SentEmails");
        public IMongoCollection<Campaign> Campaigns => _database.GetCollection<Campaign>("Campaigns");
        public IMongoCollection<Attachment> Attachments => _database.GetCollection<Attachment>("Attachments");
        public IMongoCollection<ScheduledEmail> ScheduledEmails => _database.GetCollection<ScheduledEmail>("ScheduledEmails");
    }
}