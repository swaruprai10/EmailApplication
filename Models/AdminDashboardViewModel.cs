namespace EmailCampaignApp.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalContacts { get; set; } // Total number of contacts in the database
        public int TotalSentEmails { get; set; }
        public int TotalUsers { get; set; }
        public List<ApplicationUser> Users { get; set; }
    }
}
