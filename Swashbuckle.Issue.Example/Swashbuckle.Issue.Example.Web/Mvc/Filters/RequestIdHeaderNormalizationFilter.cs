using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Swashbuckle.Issue.Example.Web.Mvc.Filters
{
    /// <inheritdoc />
    /// <summary>
    ///     Ensures the request header "x-request-id" is present by searching in known valid headers, or generating one if
    ///     none is present.
    /// </summary>
    public class RequestIdHeaderNormalizationFilter : IAuthorizationFilter
    {
        private readonly ILogger _logger;

        // ReSharper disable once SuggestBaseTypeForParameter
        public RequestIdHeaderNormalizationFilter(ILogger<RequestIdHeaderNormalizationFilter> logger)
        {
            _logger = logger;
        }

        //TODO Make the list of request headers to look for configurable.
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            string requestId = context.HttpContext.Request.Headers["x-request-id"].FirstOrDefault()
                               ?? GenerateGuid();
            context.HttpContext.Request.Headers["x-request-id"] = requestId;
            context.HttpContext.Response.Headers["x-request-id"] = requestId;
        }

        private string GenerateGuid()
        {
            string generateGuid = Guid.NewGuid().ToString("N");
            _logger.LogDebug("No request id header found. Generated a new request id: {generateGuid}.", generateGuid);
            return generateGuid;
        }
    }
}
