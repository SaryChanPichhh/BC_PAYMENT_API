using Microsoft.AspNetCore.Mvc.Filters;

namespace BC.PAYMENT.API.Helper;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata
            .Any(metadata => metadata is AllowAnonymousAttribute);

        if (allowAnonymous) return; // Skip authorization

        var user = context.HttpContext.Items["User"];
        if (user == null)
            context.Result = new JsonResult(new { message = "Unauthorized Access" })
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
    }
}