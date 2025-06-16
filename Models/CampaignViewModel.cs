using System.ComponentModel.DataAnnotations;

namespace EmailCampaignApp.Models
{
    public class CampaignViewModel
    {
        [Required(ErrorMessage = "Campaign name is required.")]
        [StringLength(100, ErrorMessage = "Name must be under 100 characters.")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description must be under 500 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Template content must be provided.")]
        public string Template { get; set; }  // HTML content of the email template

        [Required(ErrorMessage = "Issue date is required.")]
        public DateTime IssueDate { get; set; }

        [Required(ErrorMessage = "Expire date is required.")]
        public DateTime ExpireDate { get; set; }
    }


}