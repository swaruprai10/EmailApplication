using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace EmailCampaignApp.Models
{
    public class Campaign
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public string TemplateContent { get; set; }  // Ensure this is not null

        [Required]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime IssueDate { get; set; }

        [Required]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime ExpireDate { get; set; }
        public bool IsSent { get; set; }

        [Url]
        [BsonIgnoreIfNull]
        public string? ImageUrl { get; set; }


        // Navigation property to link the campaign to attachments
        [BsonIgnoreIfNull]
        public List<Attachment> Attachments { get; set; } = new List<Attachment>();
    }


}