using CustomIdentityServer.Models;

namespace CustomIdentityServer.Services
{
    public class TokenService
    {
        public RefreshToken GenerateRefreshToken()
        {
            return new RefreshToken
            {
                Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                ExpiresAt = DateTime.UtcNow.AddDays(7) // 7 gün geçerli
            };
        }
    }
}
