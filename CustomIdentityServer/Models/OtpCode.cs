using System;

namespace CustomIdentityServer.Models
{
    public class OtpCode
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? Code { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
    }
}
