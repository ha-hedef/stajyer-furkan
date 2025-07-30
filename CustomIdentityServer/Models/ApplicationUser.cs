using Microsoft.AspNetCore.Identity;
using System;

namespace CustomIdentityServer.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsLocked { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastPasswordChangeAt { get; set; } = DateTime.UtcNow;
        public string MasterPin { get; set; } = string.Empty;
        public bool HasAcceptedPrivacyPolicy { get; set; } = false;

    }
}

