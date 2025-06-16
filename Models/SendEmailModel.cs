namespace EmailCampaignApp.Models
{
    public class SendEmailModel
    {
        public string Subject { get; set; }
        public string CampaignType { get; set; }

        // The email body content
        public List<Attachment> Attachments { get; set; } = new List<Attachment>();
    }
}