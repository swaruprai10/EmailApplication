namespace EmailCampaignApp.Models
{
    public class ScheduledEmailsByDateViewModel
    {
        public string Date { get; set; }
        public List<EmailStatusViewModel> Emails { get; set; }
    }

    public class EmailStatusViewModel
    {
        public string RecipientEmail { get; set; }
        public string Status { get; set; }
    }
}