using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EmailCampaignApp.Models
{
    public class Attachment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string CampaignId { get; set; }  // Foreign key to the Campaign
        public string FileName { get; set; }
        public string FilePath { get; set; }

        // Navigation property to link the attachment to a campaign
        // Remove navigation property for MongoDB or store campaign Id only
        // public Campaign Campaign { get; set; } 
    }
}
