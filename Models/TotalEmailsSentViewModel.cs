namespace EmailCampaignApp.Models
{
    public class TotalEmailsSentViewModel
    {
        public List<Email> Emails { get; set; }
    }

    public class Email
    {
        public DateTime SentDate { get; set; }
        public string RecipientEmail { get; set; }
        public string Status { get; set; }

        // Convert to Nepal Time Zone (NPT)
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