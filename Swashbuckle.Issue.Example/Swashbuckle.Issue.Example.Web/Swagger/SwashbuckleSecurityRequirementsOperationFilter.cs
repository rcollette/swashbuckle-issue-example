using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using ZNetCS.AspNetCore.Authentication.Basic;

// TODO - Swagger code should be put in its own nuget package
namespace Swashbuckle.Issue.Example.Web.Swagger
{
    /// <summary>
    ///     Looks for AuthenticationSchemes being applied to methods and adds appropriate documentation and response codes.
    /// </summary>
    /// <remarks>
    ///     <p>
    ///         Handles JwtBearer and Cookie AuthenticationSchemes.
    ///     </p>
    ///     <p>
    ///         If a recognized scheme is found, adds an appropriate swagger security scheme and 401 and 403 as possible
    ///         HTTP response codes.
    ///     </p>
    /// </remarks>
    public class SwashbuckleSecurityRequirementsOperationFilter : IOperationFilter
    {
        private readonly ILogger<SwashbuckleSecurityRequirementsOperationFilter> _logger;

        public SwashbuckleSecurityRequirementsOperationFilter(
            ILogger<SwashbuckleSecurityRequirementsOperationFilter> logger)
        {
            _logger = logger;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Get the authorization attribute
            if (!context.ApiDescription.TryGetMethodInfo(out MethodInfo methodInfo))
            {
                return;
            }

            AuthorizeAttribute authorization = methodInfo.GetCustomAttributes(typeof(AuthorizeAttribute), true)
                .OfType<AuthorizeAttribute>().SingleOrDefault();

            if (authorization == null)
            {
                return;
            }

            string[] schemes = authorization.AuthenticationSchemes.Split(",");
            List<OpenApiSecurityRequirement> openApiSecurityRequirements = new List<OpenApiSecurityRequirement>();
            foreach (string scheme in schemes)
            {
                switch (scheme.Trim())
                {
                    //Avoid Basic authentication, but if you absolutely must....
                    case BasicAuthenticationDefaults.AuthenticationScheme:
                        _logger.LogDebug("Adding OpenApiSecurityRequirement for scheme:" +
                                         BasicAuthenticationDefaults.AuthenticationScheme + " to operation: " +
                                         operation.OperationId);
                        openApiSecurityRequirements.Add(new OpenApiBasicAuthSecurityRequirement());
                        break;
                    case JwtBearerDefaults.AuthenticationScheme:
                        _logger.LogDebug("Adding OpenApiSecurityRequirement for scheme:" +
                                         JwtBearerDefaults.AuthenticationScheme + " to operation: " +
                                         operation.OperationId);
                        openApiSecurityRequirements.Add(new OpenApiJwtBearerSecurityRequirement());
                        break;
                    case CookieAuthenticationDefaults.AuthenticationScheme:
                        _logger.LogDebug("Adding OpenApiSecurityRequirement for scheme:" +
                                         CookieAuthenticationDefaults.AuthenticationScheme + " to operation: " +
                                         operation.OperationId);
                        openApiSecurityRequirements.Add(new OpenApiJwtCookieSecurityRequirement());
                        break;
                }
            }

            if (!openApiSecurityRequirements.Any())
            {
                return;
            }

            operation.Security = openApiSecurityRequirements;
            operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });
        }
    }
}
