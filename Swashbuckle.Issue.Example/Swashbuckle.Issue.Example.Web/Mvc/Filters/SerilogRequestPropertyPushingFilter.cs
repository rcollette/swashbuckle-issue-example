using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog.Context;

namespace Swashbuckle.Issue.Example.Web.Mvc.Filters
{
    /// <summary>
    ///     Adds the x-request-id header value to the XRequestId property of the Serilog <see cref="LogContext" />.
    /// </summary>
    public class SerilogRequestPropertyPushingFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            LogContext.PushProperty("XRequestId", context.HttpContext.Request.Headers["x-request-id"].First());
        }
    }
}
