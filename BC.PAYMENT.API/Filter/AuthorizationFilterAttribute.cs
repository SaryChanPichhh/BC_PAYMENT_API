using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BC.PAYMENT.API.Filter
{
    public class AuthorizationFilterAttribute : Attribute, IAuthorizationFilter
    {
        #region ===[ Private Members ]=============================================================

        private readonly string _key;

        #endregion

        #region ===[ Constructor ]=================================================================

        public AuthorizationFilterAttribute(IConfiguration configuration)
        {
            _key = configuration["Jwt:PYS_Key"];
            
        }

        #endregion

        #region ===[ Public Methods ]==============================================================
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Retrieve the Authorization header
            var authHeader = context.HttpContext.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(authHeader))
            {
                SetUnauthorizedResult(context, "Authorization header is missing.");
                return;
            }

            // Validate "Bearer" or raw API key
            if (!IsValidApiKey(authHeader))
            {
                SetUnauthorizedResult(context, "Invalid API key provided.");
            }
        }

        private bool IsValidApiKey(string authHeader)
        {
            authHeader = authHeader?.Trim();
            // Check for "Bearer <token>" format
            if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                return token == _key;
            }

            // Check if the raw key matches
            return authHeader.Equals(_key, StringComparison.OrdinalIgnoreCase) ;
        }

        private void SetUnauthorizedResult(AuthorizationFilterContext context, string message)
        {
            context.Result = new JsonResult(new { Message = message }) { StatusCode = 401 };
        }

            #region Commentted

            //public void OnAuthorization(AuthorizationFilterContext context)
            //{
            //    var apiKeyHeader = context.HttpContext.Request.Headers["Authorization"].ToString();
            //    var authController = new Controllers.AuthController();

            //    if (apiKeyHeader.Any())
            //    {
            //        var keys = new List<string>
            //        {
            //            _key
            //        };

            //        if (keys.FindIndex(x => x.Equals(apiKeyHeader, StringComparison.OrdinalIgnoreCase)) == -1)
            //        {
            //            context.Result = authController.NotAuthorized();
            //        }
            //    }
            //    else
            //    {
            //        context.Result = authController.NotAuthorized();
            //    }
            //}
            

            #endregion

        #endregion
    }
}
