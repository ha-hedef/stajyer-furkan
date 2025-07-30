using System.ComponentModel.DataAnnotations;
namespace CustomIdentityServer.Models.DTOs
{

    public class OtpRequestDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;
    }
}
