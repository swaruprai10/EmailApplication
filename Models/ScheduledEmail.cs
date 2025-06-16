using MongoDB.Bson.Serialization.Attributes;

namespace EmailCampaignApp.Models
{
    public class ScheduledEmail
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }

        public string CampaignName { get; set; }
        public string RecipientEmail { get; set; }
        public DateTime ScheduledDate { get; set; }


        // Convert to Nepal Time Zone (NPT)
        public DateTime LocalScheduledDate
        {
            get
            {
                var nepalTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Nepal Standard Time");
                return TimeZoneInfo.ConvertTimeFromUtc(ScheduledDate, nepalTimeZone);
            }
        }
    }
}