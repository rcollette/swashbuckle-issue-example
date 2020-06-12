using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.OpenApi.Models;

namespace Swashbuckle.Issue.Example.Web.Swagger
{
    public class OpenApiJwtCookieSecurityRequirement : OpenApiSecurityRequirement
    {
        /// <summary>
        ///     Open API 3.0 JWT Cookie Authorization.
        /// </summary>
        /// <param name="cookieName">The name of the cookie in which the JWT formatted api key will be stored.</param>
        /// <remarks>
        ///     <see href="https://github.com/OAI/OpenAPI-Specification/blob/master/versions/3.0.0.md#jwt-bearer-sample" />.
        /// </remarks>
        public OpenApiJwtCookieSecurityRequirement(string cookieName = "api_key")
        {
            Add(
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Name = cookieName,
                    In = ParameterLocation.Cookie,
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = CookieAuthenticationDefaults.AuthenticationScheme
                    }
                },
                // list of string scopes is required for something like OAuth
                new List<string>());
        }
    }
}
