using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using CustomIdentityServer.Data;
using CustomIdentityServer.Models;
using CustomIdentityServer.Models.DTOs;
using CustomIdentityServer.Services;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Newtonsoft.Json;

namespace CustomIdentityServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly OtpService _otpService;
        private readonly IEmailService _emailService; // Eksik alan eklendi
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly TokenService _tokenService; // Eksik alan eklendi

        public AuthController(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            OtpService otpService,
            AppDbContext context,
            IEmailService emailService,
            RoleManager<IdentityRole> roleManager,// Eklendi
            TokenService tokenService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _otpService = otpService;
            _context = context;
            _emailService = emailService;
            _roleManager = roleManager; // Eklendi
            _tokenService = tokenService;
            
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(model.MasterPin))
                return BadRequest("MasterPin is required.");

            if (!_roleManager.RoleExistsAsync("User").Result)
            {
                await _roleManager.CreateAsync(new IdentityRole("User"));
            }
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                MasterPin = BCrypt.Net.BCrypt.HashPassword(model.MasterPin)
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            var roleResult = await _userManager.AddToRoleAsync(user, "User");
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user); // Rollback
                return StatusCode(500, "User could not be assigned to role. Registration rolled back.");
            }

            // E-posta doğrulama tokeni oluştur ve gönder
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmLink = $"https://example.com/confirm-email?email={user.Email}&token={Uri.EscapeDataString(token)}";
            var subject = "E-posta Doğrulama";
            var body = $"E-posta adresinizi doğrulamak için tıklayın: {confirmLink}";
            try
            {
                if (string.IsNullOrEmpty(user.Email))
                    return StatusCode(500, "User email address is missing.");
                await _emailService.SendEmailAsync(user.Email, subject, body);
            }
            catch (Exception)
            {
                // Hata loglanabilir
                return StatusCode(500, "Email could not be sent.");
            }
            if (string.IsNullOrEmpty(user.Email))
                return StatusCode(500, "User email address is missing.");
            await _emailService.SendEmailAsync(user.Email, subject, body);

            return Ok("User created successfully. Please check your email to confirm your address.");
        }

        [HttpPost("verify-pin")]
        public async Task<IActionResult> VerifyPin([FromBody] VerifyPinDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(dto.Pin))
                return BadRequest("PIN is required.");

            var user = await _userManager.FindByNameAsync(dto.Username);
            if (user == null)
                return NotFound("User not found");

            if (!BCrypt.Net.BCrypt.Verify(dto.Pin, user.MasterPin))
                return Unauthorized("Invalid PIN");

            return Ok("PIN verified successfully");
        }

        // 2FA: Login Step 1 - Kullanıcı adı ve şifre kontrolü, OTP gönderimi
        [HttpPost("login-step1")]
        public async Task<IActionResult> LoginStep1([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
                return Unauthorized("Invalid credentials");
            if (!user.EmailConfirmed)
                return Unauthorized("You must confirm your email address before logging in.");
            if (!user.HasAcceptedPrivacyPolicy)
                return Unauthorized("You must accept the privacy policy before logging in.");
            if (user.LockoutEnabled && user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow)
            {
                var minutesLeft = (int)(user.LockoutEnd.Value - DateTimeOffset.UtcNow).TotalMinutes;
                return Unauthorized($"Account is locked. Try again in {minutesLeft} minutes.");
            }
            var maxAttempts = int.Parse(_configuration["Security:MaxFailedLoginAttempts"] ?? "5");
            var lockMinutes = int.Parse(_configuration["Security:AccountLockDurationMinutes"] ?? "15");
            var since = DateTime.UtcNow.AddMinutes(-lockMinutes);
            var failedAttempts = _context.LoginAttemptLogs.Count(l => l.UserId == user.Id && !l.IsSuccessful && l.AttemptTime > since);
            if (failedAttempts >= maxAttempts)
            {
                user.LockoutEnabled = true;
                user.LockoutEnd = DateTimeOffset.UtcNow.AddMinutes(lockMinutes);
                await _userManager.UpdateAsync(user);
                return Unauthorized($"Account is locked due to too many failed attempts. Try again in {lockMinutes} minutes.");
            }
            if (!await _userManager.CheckPasswordAsync(user, model.Password))
            {
                _context.LoginAttemptLogs.Add(new LoginAttemptLog
                {
                    UserId = user.Id,
                    IsSuccessful = false,
                    AttemptTime = DateTime.UtcNow
                });
                await _context.SaveChangesAsync();
                return Unauthorized("Invalid credentials");
            }
            // OTP gönder
            var code = await _otpService.GenerateOtpAsync(user.Id);

            var subject = "Giriş için OTP";
            var body = $"Girişinizi tamamlamak için OTP kodunuz: {code}";
            if (string.IsNullOrEmpty(user.Email))
                return StatusCode(500, "User email address is missing.");
            await _emailService.SendEmailAsync(user.Email!, subject, body);
            return Ok("OTP sent to your email address. Please verify to complete login.");
        }

        // 2FA: Login Step 2 - OTP doğrulama ve token üretimi
        [HttpPost("login-step2")]
        public async Task<IActionResult> LoginStep2([FromBody] OtpVerifyDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByNameAsync(dto.Username);
            if (user == null)
                return Unauthorized("User not found");

            var isValid = await _otpService.VerifyOtpAsync(user.Id, dto.Code);
            if (!isValid)
                return Unauthorized("Invalid or expired OTP");

            // Başarılı giriş logla
            _context.LoginAttemptLogs.Add(new LoginAttemptLog
            {
                UserId = user.Id,
                IsSuccessful = true,
                AttemptTime = DateTime.UtcNow
            });

            var oldFailed = _context.LoginAttemptLogs.Where(l => l.UserId == user.Id && !l.IsSuccessful);
            _context.LoginAttemptLogs.RemoveRange(oldFailed);
            await _context.SaveChangesAsync();

            if (user.LockoutEnabled || user.LockoutEnd != null)
            {
                user.LockoutEnabled = false;
                user.LockoutEnd = null;
                await _userManager.UpdateAsync(user);
            }

            // Access Token üret
            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(jwtKey))
                return StatusCode(500, "JWT key is not configured.");

            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            foreach (var role in userRoles)
                authClaims.Add(new Claim(ClaimTypes.Role, role));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var token = new JwtSecurityToken(
                expires: DateTime.Now.AddMinutes(15), // Access Token süresi kısa
                claims: authClaims,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            // ✅ Refresh Token üret ve DB’ye kaydet
            var refreshToken = _tokenService.GenerateRefreshToken();
            refreshToken.UserId = user.Id;

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                accessToken = new JwtSecurityTokenHandler().WriteToken(token),
                accessTokenExpiration = token.ValidTo,
                refreshToken = refreshToken.Token
            });
        }


        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] OtpRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = await _userManager.FindByNameAsync(dto.Username);
            if (user == null)
                return NotFound("User not found");

            var code = await _otpService.GenerateOtpAsync(user.Id);

            var subject = "OTP Kodu";
            var body = $"Giriş veya işlem için OTP kodunuz: {code}";
            try
            {
                if (string.IsNullOrEmpty(user.Email))
                    return BadRequest("User does not have a valid email.");

                await _emailService.SendEmailAsync(user.Email, subject, body);

            }
            catch (Exception)
            {
                // Hata loglanabilir
                return StatusCode(500, "OTP email could not be sent.");
            }

            return Ok("OTP sent to your email address.");
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpVerifyDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = await _userManager.FindByNameAsync(dto.Username);
            if (user == null)
                return NotFound("User not found");

            var isValid = await _otpService.VerifyOtpAsync(user.Id, dto.Code);
            if (!isValid)
                return Unauthorized("Invalid or expired OTP");

            return Ok("OTP verified successfully");
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return NotFound("Email not found");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"https://example.com/reset-password?email={user.Email}&token={Uri.EscapeDataString(token)}";

            var subject = "Şifre Sıfırlama";
            var body = $"Şifrenizi sıfırlamak için tıklayın: {resetLink}";
            try
            {
                await _emailService.SendEmailAsync(user.Email!, subject, body);
            }
            catch (Exception)
            {
                return StatusCode(500, "Password reset email could not be sent.");
            }

            return Ok("Password reset link sent via email.");
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] string refreshToken)
        {
            var storedToken = _context.RefreshTokens.FirstOrDefault(t => t.Token == refreshToken);

            if (storedToken == null || storedToken.ExpiresAt < DateTime.UtcNow || storedToken.IsRevoked)
                return Unauthorized("Invalid or expired refresh token");

            var userId = storedToken.UserId;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Invalid refresh token: missing userId");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Unauthorized("User not found");

            // Eski refresh token iptal
            storedToken.IsRevoked = true;

            // Yeni refresh token oluştur
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            newRefreshToken.UserId = user.Id;
            _context.RefreshTokens.Add(newRefreshToken);

            // Yeni Access Token oluştur
            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
                return StatusCode(500, "JWT key is missing in configuration.");

            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var token = new JwtSecurityToken(
                expires: DateTime.UtcNow.AddHours(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            await _context.SaveChangesAsync();

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo,
                refreshToken = newRefreshToken.Token
            });
        }



        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return NotFound("User not found");

            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            user.LastPasswordChangeAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            return Ok("Password has been reset successfully."); // Eksik return eklendi
        }

        [HttpPost("accept-privacy-policy")]
        public async Task<IActionResult> AcceptPrivacyPolicy([FromBody] PrivacyPolicyConsentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = await _userManager.FindByNameAsync(dto.Username);
            if (user == null)
                return NotFound("User not found");

            user.HasAcceptedPrivacyPolicy = dto.Accepted;
            await _userManager.UpdateAsync(user);

            var consent = _context.PrivacyPolicyConsents
                .FirstOrDefault(x => x.UserId == user.Id);
            if (consent == null)
            {
                consent = new PrivacyPolicyConsent
                {
                    UserId = user.Id,
                    Accepted = dto.Accepted,
                    AcceptedAt = DateTime.UtcNow
                };
                _context.PrivacyPolicyConsents.Add(consent);
            }
            else
            {
                consent.Accepted = dto.Accepted;
                consent.AcceptedAt = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync();

            return Ok("Privacy policy status recorded.");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnly()
        {
            return Ok("You are an admin!");
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetProfile()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Unauthorized();
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound("User not found");
            return Ok(new
            {
                user.UserName,
                user.Email,
                user.EmailConfirmed,
                user.CreatedAt,
                user.HasAcceptedPrivacyPolicy,
                user.LockoutEnabled,
                user.LockoutEnd
            });
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return NotFound("User not found");
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
                return Ok("Email confirmed successfully.");
            return BadRequest("Invalid or expired token.");
        }
    }
}



