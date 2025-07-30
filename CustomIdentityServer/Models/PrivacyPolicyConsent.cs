using System;

namespace CustomIdentityServer.Models
{
    public class PrivacyPolicyConsent
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public bool Accepted { get; set; }
        public DateTime AcceptedAt { get; set; }
    }
}
