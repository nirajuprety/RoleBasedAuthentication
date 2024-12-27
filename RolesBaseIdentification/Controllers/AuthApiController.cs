using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Authenticator;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RolesBaseIdentification.Model.DTOs.Request;

namespace RolesBaseIdentification.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthApiController:ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthApiController(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
           
            if (user != null && await _userManager.CheckPasswordAsync(user, request.Password))
            {
                if (string.IsNullOrEmpty(request.OtpCode))
                {
                    return BadRequest("OTP code is required for 2FA.");
                }
                if (!request.IsEmailedOtp)
                {
                    var secretKey = await _userManager.GetAuthenticationTokenAsync(user, "TOTP", "2FA-Secret");

                    var authenticator = new TwoFactorAuthenticator();
                    var pinCode = authenticator.GetCurrentPIN(secretKey);
                    if (pinCode == request.OtpCode)
                    {
                        var result = authenticator
                        .ValidateTwoFactorPIN(secretKey, request.OtpCode);


                    }
                    else
                    {
                        return Unauthorized("Invalid OTP code.");
                    }
                }
                else
                {
                    var emailOtp = await _userManager.GetAuthenticationTokenAsync(user, "Email2FA", "2FA-OTP");
                    if (string.IsNullOrEmpty(emailOtp))
                    {
                        return BadRequest("Email OTP is required, but none was sent. Please request a new OTP.");
                    }

                    if (request.OtpCode != emailOtp)
                    {
                        return Unauthorized("Invalid email OTP.");
                    }

                    await _userManager.RemoveAuthenticationTokenAsync(user, "Email2FA", "2FA-OTP");
                }
  

                var userRoles = await _userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

                var token = GetToken(authClaims);
                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token), expiration = token.ValidTo });
            }
            return Unauthorized();
        }
        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return token;
        }

        [HttpGet("IsTwoFactorEnabled")]
        public async Task<IActionResult> IsTwoFactorEnabled(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new { success = false, message = "Email is required." });
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return NotFound(new { success = false, message = "User not found." });
            }

            var isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
            return Ok(new { success = true, isTwoFactorEnabled });
        }

        [HttpPost("SimpleLogin")]
        public async Task<IActionResult> SimpleLogin([FromBody] UserRequest userRequest)
        {
            var user = await _userManager.FindByEmailAsync(userRequest.Email);

            if (user != null && await _userManager.CheckPasswordAsync(user, userRequest.Password))
            {

                var userRoles = await _userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

                var token = GetToken(authClaims);
                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token), expiration = token.ValidTo });
            }
            return Unauthorized();
        }
        [HttpPost("VerifyEnteredOtp")]
        public async Task<IActionResult> VerifyEnteredOtp([FromBody] VerifyOtpRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.OtpCode))
            {
                return BadRequest(new { success = false, message = "Email and OTP code are required." });
            }

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return NotFound(new { success = false, message = "User not found." });
            }

            if (request.IsEmailedOtp)
            {
                var emailOtp = await _userManager.GetAuthenticationTokenAsync(user, "Email2FA", "2FA-OTP");
                if (string.IsNullOrEmpty(emailOtp))
                {
                    return BadRequest(new { success = false, message = "No OTP has been sent via email. Please request a new OTP." });
                }

                if (request.OtpCode != emailOtp)
                {
                    return Unauthorized(new { success = false, message = "Invalid email OTP." });
                }

                await _userManager.RemoveAuthenticationTokenAsync(user, "Email2FA", "2FA-OTP");
            }
            else
            {
                var secretKey = await _userManager.GetAuthenticationTokenAsync(user, "TOTP", "2FA-Secret");
                if (string.IsNullOrEmpty(secretKey))
                {
                    return BadRequest(new { success = false, message = "TOTP authentication is not configured for this user." });
                }

                var authenticator = new TwoFactorAuthenticator();
                if (!authenticator.ValidateTwoFactorPIN(secretKey, request.OtpCode))
                {
                    return Unauthorized(new { success = false, message = "Invalid TOTP code." });
                }
            }

            return Ok(new { success = true, message = "OTP verified successfully." });
        }
    }
}
