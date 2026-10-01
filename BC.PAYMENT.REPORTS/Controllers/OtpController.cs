using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BC.ACCOUNTING.REPORT.Models;
using BC.ACCOUNTING.REPORT.Services;
using BC.PAYMENT.APPLICATION.Interfaces.Login;
using BC.PAYMENT.REPORTS.Helper;
using BC.PAYMENT.REPORTS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BC.PAYMENT.REPORTS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OtpController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private static ConcurrentDictionary<string, OtpEntry> _otpStore = new();
    private readonly IUserRepository _userRepository;

    public OtpController(IConfiguration configuration, IUserRepository userRepository)
    {
        _configuration = configuration;
        _userRepository = userRepository;
    }

    [HttpPost("request")]
    public async Task<IActionResult> RequestOtp([FromBody] OtpRequestModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Username))
            return BadRequest(new { success = false, message = "Username is required." });

        var userId = await _userRepository.GetUserForOTP(model.Username);
        if (userId == null)
            return NotFound(new { success = false, message = "Username not found." });


        var otpEntry = ReportExtension.GenerateSecureOtp();
        _otpStore[model.Username] = otpEntry;

        await TelegramOtpSender.SendOtpToTelegram(otpEntry.OtpCode, model.Username);

        return Ok(new { success = true });
    }

    [HttpPost("verify")]
    public IActionResult VerifyOtp([FromBody] OtpVerifyModel model)
    {
        if (!_otpStore.TryGetValue(model.Username, out var entry))
            return Unauthorized(new { success = false, message = "OTP not found." });

        if (entry.OtpCode != model.Otp || (DateTime.UtcNow - entry.CreatedAt).TotalMinutes > 1)
            return Unauthorized(new { success = false, message = "Invalid or expired OTP." });

        _otpStore.TryRemove(model.Username, out _); // remove used OTP


        var token = GenerateJwtToken(model.Username);
        Response.Cookies.Append("AuthToken", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddMinutes(30)
        });
        return Ok(new { success = true });
    }

    private string GenerateJwtToken(string username)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("Username", username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}