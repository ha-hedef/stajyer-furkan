using System.ComponentModel.DataAnnotations;
namespace CustomIdentityServer.Models.DTOs
{

    public class OtpVerifyDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Otp { get; set; } = string.Empty;

        [Required]
        public string Code { get; set; } = string.Empty;
    }
}
