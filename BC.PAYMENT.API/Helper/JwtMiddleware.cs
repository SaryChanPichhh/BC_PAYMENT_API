using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.General;
using BC.PAYMENT.LOGGING;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace BC.PAYMENT.API.Helper
{
    public class JwtMiddleware
    {
        #region ===[ Private Members ]=============================================================

        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;
        #endregion

        #region ===[ Constructor ]=================================================================

        public JwtMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
        {
            _next = next;
            _appSettings = appSettings.Value;
        }

        #endregion

        #region ===[ Public Methods ]==============================================================

        public async Task Invoke(HttpContext context, IUnitOfWork unitOfWork)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                AttachUserToContext(context, unitOfWork, token);

            await _next(context);
        }

        #endregion

        #region ===[ Private Methods ]=============================================================

        private void AttachUserToContext(HttpContext context, IUnitOfWork unitOfWork, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.PYS_Key);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _appSettings.Issuer,
                    ValidAudience = _appSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                var contextDto = new ContextDTO
                {
                    UserId = int.Parse(jwtToken.Claims.First(x => x.Type == "UserId").Value),
                    DbCode = jwtToken.Claims.First(x => x.Type == "DbCode").Value,
                    AppCode = jwtToken.Claims.First(x => x.Type == "AppCode").Value
                };


                context.Items["User"] = unitOfWork.Users.GetUserByIdAsync(contextDto).Result;
            }
            catch (SecurityTokenException ex)
            {
                // Token validation failed
                Logger.Instance.Error("Token validation failed:", ex);
            }
            catch (Exception ex)
            {
                // Log unexpected errors
                Logger.Instance.Error("An error occurred while attaching user to context", ex);
            }
        }

        #endregion
    }
}

