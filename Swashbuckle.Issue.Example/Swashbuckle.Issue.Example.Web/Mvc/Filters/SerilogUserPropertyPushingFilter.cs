using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog.Context;

namespace Swashbuckle.Issue.Example.Web.Mvc.Filters
{
    /// <summary>
    ///     An IActionFilter that put User information into the Serilog <see cref="LogContext" />.
    /// </summary>
    /// <remarks>
    ///     Pushes a UserName property with a value of <see cref="HttpContext.User" /> and
    ///     a UserClaims property that is a dictionary of the configured ClaimsToInclude.
    /// </remarks>
    public class SerilogUserPropertyPushingFilter : IActionFilter
    {
        //TODO Make configurable
        private static readonly string[] _claimsToInclude = { "idpUserId", "UserEmail", "ClientId" };

        public void OnActionExecuting(ActionExecutingContext context)
        {
            string? name = context.HttpContext.User?.Identity.Name;
            if (name != null)
            {
                LogContext.PushProperty("UserName", name);
            }

            IEnumerable<Claim>? claims = context.HttpContext.User?.Claims;
            if (claims != null)
            {
                LogContext.PushProperty(
                    "UserClaims",
                    claims.Where(c => _claimsToInclude.Contains(c.Type))
                        .ToDictionary(c => c.Type, c => c.Value));
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
