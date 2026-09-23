using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BC.PAYMENT.CORE.Contracts.General;
using BC.PAYMENT.CORE.DTO.General;

namespace BC.PAYMENT.API.Helper
{
    public static class Common
    {
        #region ===[ Public Methods ]==============================================================

        /// <summary>
        /// Generate Jwt Token
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="appSettingSecret"></param>
        /// <returns></returns>
        public static string GenerateJwtToken(int userId, AppSettings appSettings)
        {
            var tokenHandler = new JwtSecurityTokenHandler(); 
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", userId.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

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
                    new Claim("InvoiceEntryCode", request.InvoiceEntryCode),
                    new Claim("Role", request.Role),
                };
           
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.PYS_Key));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            
                var tokenDescriptor = new JwtSecurityToken(
                    appSettings.Issuer,
                    appSettings.Audience,
                    claims,
                    expires: DateTime.UtcNow.AddDays(1000),
                    signingCredentials: signIn
                );
                var token = tokenHandler.WriteToken(tokenDescriptor);
                return (token);
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
                Period = user.FindFirst("Period")?.Value ?? $"{DateTime.Now.Year}{DateTime.Now.Month:D2}"
            };
        }


        #endregion
    }
}
