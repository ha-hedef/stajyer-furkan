using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CustomIdentityServer.Data;
using CustomIdentityServer.Models;
using CustomIdentityServer.Services;


namespace CustomIdentityServer.Services
{
    public class OtpService
    {
        private readonly AppDbContext _context;
        private readonly TimeSpan _otpExpiry = TimeSpan.FromMinutes(5);

        public OtpService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateOtpAsync(string userId)
        {
            var code = new Random().Next(100000, 999999).ToString();

            var otp = new OtpCode
            {
                UserId = userId,
                Code = code,
                ExpiresAt = DateTime.UtcNow.Add(_otpExpiry),
                IsUsed = false
            };

            _context.OtpCodes.Add(otp);
            await _context.SaveChangesAsync();

            return code;
        }

        public async Task<bool> VerifyOtpAsync(string userId, string code)
        {
            var otp = await _context.OtpCodes
                .Where(o => o.UserId == userId && o.Code == code && !o.IsUsed && o.ExpiresAt > DateTime.UtcNow)
                .FirstOrDefaultAsync();

            if (otp == null)
                return false;

            otp.IsUsed = true;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
