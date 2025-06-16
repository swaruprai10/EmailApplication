namespace EmailCampaignApp.Models
{
    public class ManageRolesViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public List<string> UserRoles { get; set; } // This should be List<string>
        public List<string> AllRoles { get; set; }
        public List<string> SelectedRoles { get; set; }
    }
}