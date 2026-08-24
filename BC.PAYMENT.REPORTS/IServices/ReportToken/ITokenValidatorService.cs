using System.Security.Claims;

namespace BC.PAYMENT.REPORTS.IServices.ReportToken
{
    public interface ITokenValidatorService
    {
        ClaimsPrincipal ValidateJwtFromCookie(HttpRequest request);

    }
}
