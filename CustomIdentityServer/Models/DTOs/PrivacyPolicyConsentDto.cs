using System.ComponentModel.DataAnnotations;
namespace CustomIdentityServer.Models.DTOs
{

    public class PrivacyPolicyConsentDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public bool Accepted { get; set; }
    }
}
