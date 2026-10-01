using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE;
using BC.PAYMENT.CORE.DTO.General;
using BC.PAYMENT.CORE.Entities.Login;
using BC.PAYMENT.LOGGING;

namespace BC.PAYMENT.API.Helper;

public class JwtMiddlewares(RequestDelegate next)
{
    public async Task Invoke(HttpContext context, IUnitOfWork unitOfWork)
    {
        try
        {
            // Check if the user is authenticated (UseAuthentication populated HttpContext.User)
            var user = context.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                // Extract claims from HttpContext.User
                var claims = Common.DecodeJwt(user);
                // Checking Is Admin (admin settings may be missing from config, so compare null-safely)
                User userData;
                var isAdmin = !string.IsNullOrEmpty(Singleton.Instance.AppCode) &&
                              !string.IsNullOrEmpty(Singleton.Instance.Username) &&
                              string.Equals(Singleton.Instance.AppCode, claims.AppCode) &&
                              string.Equals(Singleton.Instance.Username, claims.Username);
                if (isAdmin)
                    userData = new User
                    {
                        UserId = 0,
                        Username = claims.Username,
                        DbCode = claims.DbCode,
                        Role = claims.Role,
                        Name = claims.Username,
                        CurrentDate = DateTime.Today
                    };
                else
                    userData = await unitOfWork.Users.GetUserByIdAsync(new ContextDTO
                    {
                        UserId = claims.UserId,
                        DbCode = claims.DbCode,
                        AppCode = claims.AppCode
                    });
                // Fetch additional user data from the database
                if (userData != null)
                    // Attach user object to HttpContext.Items for downstream access
                    context.Items["User"] = userData;
            }
        }
        catch (Exception ex)
        {
            // Log any errors
            Logger.Instance.Error("An error occurred in JwtMiddleware:", ex);
        }

        // Continue with the pipeline
        await next(context);
    }
}