namespace EmailApplication.Models
{
    public class SendNowRequest
    {
        public string Id { get; set; }             //CampaignId
        public List<string> EmailIds { get; set; } //List of contact ids
    }
}
