using System.Text;
using Google.Authenticator;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RolesBaseIdentification.Model.DTOs.Response;

namespace RolesBaseIdentification.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticatorApiController:ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthenticatorApiController(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }
        [HttpGet("SetTOTP")]
        public async Task<bool> SetTOTP(string userName)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userName);

                string secretKey = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 32); 
                var result = await _userManager.SetAuthenticationTokenAsync(user, "TOTP", "2FA-Secret", secretKey);

                return result != null;
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

    }
}
