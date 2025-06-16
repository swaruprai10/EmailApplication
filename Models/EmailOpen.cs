using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EmailCampaignApp.Models
{
    public class EmailOpen
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }


        [BsonElement("Email")]
        public string? Email { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string? CampaignId { get; set; }

        public DateTime OpenedAt { get; set; }
    }
}
