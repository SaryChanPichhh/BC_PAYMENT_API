using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BC.ACCOUNTING.REPORT.Models;
using BC.PAYMENT.CORE.Contracts.General;
using BC.PAYMENT.CORE.DTO.General;
using Microsoft.IdentityModel.Tokens;

namespace BC.PAYMENT.REPORTS.Helper;

public static class ReportExtension
{
    public static OtpEntry GenerateSecureOtp()
    {
        var bytes = new byte[4];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }

        var otp = Math.Abs(BitConverter.ToInt32(bytes, 0) % 900000 + 100000);

        return new OtpEntry
        {
            OtpCode = otp.ToString("D6"),
            CreatedAt = DateTime.UtcNow
        };
    }

    public static bool VerifyOtp(string inputOtp, OtpEntry storedOtp)
    {
        // Check expiration
        if ((DateTime.UtcNow - storedOtp.CreatedAt).TotalMinutes > 1) return false; // expired

        // Check value
        return inputOtp == storedOtp.OtpCode;
    }

    #region JWTToken

    public static string GenerateJwtToken(ClaimDTO request, AppSettings appSettings)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, appSettings.Subject),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("UserId", request.UserId.ToString()),
            new Claim("Username", request.Username!),
            new Claim("DbCode", request.DbCode!),
            new Claim("AppCode", request.AppCode!),
            new Claim("CompanyCode", request.CompanyCode!),
            new Claim("CurrentDate", request.CurrectDate.ToString("MM/dd/yyyy")),
            new Claim("InvoiceEntryCode", request.InvoiceEntryCode!)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.Key));
        var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new JwtSecurityToken(
            appSettings.Issuer,
            appSettings.Audience,
            claims,
            expires: DateTime.UtcNow.AddDays(1000),
            signingCredentials: signIn
        );
        var token = tokenHandler.WriteToken(tokenDescriptor);
        return token;
    }

    public static ClaimDTO DecodeJwt(ClaimsPrincipal user)
    {
        if (user?.Identity?.IsAuthenticated != true || !user.Claims.Any())
            throw new ArgumentException("ClaimsPrincipal does not contain valid claims.");

        return new ClaimDTO
        {
            UserId = int.TryParse(user.FindFirst("UserId")?.Value, out var userId) ? userId : 0,
            Username = user.FindFirst("Username")?.Value ?? string.Empty,
            DbCode = user.FindFirst("DbCode")?.Value ?? string.Empty,
            AppCode = user.FindFirst("AppCode")?.Value ?? string.Empty,
            CompanyCode = user.FindFirst("CompanyCode")?.Value ?? string.Empty,
            CurrectDate = DateTime.TryParse(user.FindFirst("CurrentDate")?.Value, out var currentDate)
                ? currentDate
                : DateTime.Now,
            InvoiceEntryCode = user.FindFirst("InvoiceEntryCode")?.Value ?? string.Empty,
            Period = user.FindFirst("Period")?.Value ?? string.Empty
        };
    }

    #endregion
}