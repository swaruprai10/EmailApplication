using MongoDB.Bson.Serialization.Attributes;

namespace EmailCampaignApp.Models
{
    public class SentEmail
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }

        public string RecipientEmail { get; set; }
        public DateTime SentDate { get; set; }

        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string CampaignId { get; set; }

        public string Status { get; set; }

        public bool IsOpened { get; set; }

        public DateTime LocalSentDate
        {
            get
            {
                var nepalTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Nepal Standard Time");
                return TimeZoneInfo.ConvertTimeFromUtc(SentDate, nepalTimeZone);
            }
        }
    }
}