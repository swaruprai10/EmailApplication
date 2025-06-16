namespace EmailCampaignApp.Models
{
    public class SendEmailViewModel
    {
        public string? RecipientEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<IFormFile>? Attachments { get; set; }
    }
}
