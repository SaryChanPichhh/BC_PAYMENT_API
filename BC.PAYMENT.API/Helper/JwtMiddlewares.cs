using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.General;
using BC.PAYMENT.LOGGING;

namespace BC.PAYMENT.API.Helper
{
    public class JwtMiddlewares
    {
        private readonly RequestDelegate _next;

        public JwtMiddlewares(RequestDelegate next)
        {
            _next = next;
        }

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

                    // Fetch additional user data from the database
                    var userData = await unitOfWork.Users.GetUserByIdAsync(new ContextDTO
                    {
                        UserId = claims.UserId,
                        DbCode = claims.DbCode,
                        AppCode = claims.AppCode
                    });

                    if (userData != null)
                    {
                        // Attach user object to HttpContext.Items for downstream access
                        context.Items["User"] = userData;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log any errors
                Logger.Instance.Error("An error occurred in JwtMiddleware:", ex);
            }

            // Continue with the pipeline
            await _next(context);
        }

       
    }

}
