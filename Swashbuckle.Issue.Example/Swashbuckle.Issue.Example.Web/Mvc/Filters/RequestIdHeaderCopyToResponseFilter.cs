using Microsoft.AspNetCore.Mvc.Filters;

namespace Swashbuckle.Issue.Example.Web.Mvc.Filters
{
    /// <inheritdoc />
    /// <summary>
    ///     Copies the "x-request-id" header value from the request headers to the response headers.
    /// </summary>
    public class RequestIdHeaderCopyToResponseFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            context.HttpContext.Response.Headers["x-request-id"] = context.HttpContext.Request.Headers["x-request-id"];
        }
    }
}
