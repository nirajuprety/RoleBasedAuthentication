using System.Text;
using Google.Authenticator;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OtpNet;
using RolesBaseIdentification.Model.DTOs.Response;
using RolesBaseIdentification.Service.EmailService;

namespace RolesBaseIdentification.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticatorApiController:ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        public AuthenticatorApiController(UserManager<IdentityUser> userManager, IConfiguration configuration, IEmailService emailService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _emailService = emailService;
        }
        [HttpGet("SetTOTP")]
        public async Task<bool> SetTOTP(string userName)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userName);

                string secretKey = GenerateSecretKey(); 
                var result = await _userManager.SetAuthenticationTokenAsync(user, "TOTP", "2FA-Secret", secretKey);

                user.TwoFactorEnabled = true;
                var updateResult = await _userManager.UpdateAsync(user);

                return updateResult.Succeeded && result != null;
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet("EnableTOTP")]
        public async Task<EnableAuthResponse> EnableTOTP(string userName)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userName);

                var secretKey = await _userManager.GetAuthenticationTokenAsync(user, "TOTP", "2FA-Secret");


                var authenticator = new TwoFactorAuthenticator();
                var setupCode = authenticator.GenerateSetupCode("MyApp", user.Email, Encoding.ASCII.GetBytes(secretKey));

                user.TwoFactorEnabled = true;
                var updateResult = await _userManager.UpdateAsync(user);
                
                return new EnableAuthResponse
                {
                    QRCode = setupCode.QrCodeSetupImageUrl,
                    EntryKey = setupCode.ManualEntryKey
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet("VerifyTOTP")]
        public async Task<bool> VerifyTOTP(string userName, string code)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userName);
                var secretKey = await _userManager.GetAuthenticationTokenAsync(user, "TOTP", "2FA-Secret");

                var authenticator = new TwoFactorAuthenticator();
                return authenticator.ValidateTwoFactorPIN(secretKey, code);
            }
            catch (Exception)
            {
                return false;
            }
        }


        [HttpPost("send-email-otp")]
        public async Task<IActionResult> SendEmailOtp(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var otp = new Random().Next(100000, 999999).ToString();

            await _userManager.SetAuthenticationTokenAsync(user, "Email2FA", "2FA-OTP", otp);

            var subject = "Your Login OTP Code";
            var message = $"Your OTP code for login is: {otp}. This code is valid for 5 minutes.";

            await _emailService.SendEmailAsync(user.Email, subject, message);

            return Ok("OTP sent successfully.");
        }
        string GenerateSecretKey()
        {
            var key = KeyGeneration.GenerateRandomKey(20); 
            return OtpNet.Base32Encoding.ToString(key);
        }
    }
}
