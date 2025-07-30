using System;

namespace CustomIdentityServer.Models
{
    public class LoginAttemptLog
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public bool IsSuccessful { get; set; }
        public DateTime AttemptTime { get; set; }
    }
}
