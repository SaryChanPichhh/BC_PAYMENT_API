using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BC.PAYMENT.REPORTS.IServices.ReportToken;
using Microsoft.IdentityModel.Tokens;

namespace BC.PAYMENT.REPORTS.Services.ReportToken
{
    public class TokenValidatorService :ITokenValidatorService
    {
        private readonly IConfiguration _config;

        public TokenValidatorService(IConfiguration config)
        {
            _config = config;
        }

        public ClaimsPrincipal ValidateJwtFromCookie(HttpRequest request)
        {
            var token = request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
                return null;

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _config["Jwt:Issuer"],
                    ValidAudience = _config["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_config["Jwt:Key"]!))
                };

                return handler.ValidateToken(token, validationParameters, out _);
            }
            catch
            {
                return null;
            }
        }
    }
}
