using System.ComponentModel.DataAnnotations;

namespace EmailCampaignApp.Models;

public class RegisterModel
{
    [Required]
    public string Username { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(8)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "Password must be at least 8 characters long and contain an uppercase letter, a lowercase letter, a number, and a special character.")]
    public string Password { get; set; }
}