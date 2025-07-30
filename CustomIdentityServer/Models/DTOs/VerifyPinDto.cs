using System.ComponentModel.DataAnnotations;
namespace CustomIdentityServer.Models.DTOs
{

    public class VerifyPinDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Pin { get; set; } = string.Empty;
    }
}
