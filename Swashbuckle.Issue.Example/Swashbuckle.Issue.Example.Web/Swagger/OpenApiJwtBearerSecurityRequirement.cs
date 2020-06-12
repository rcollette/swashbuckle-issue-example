using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace Swashbuckle.Issue.Example.Web.Swagger
{
    public class OpenApiJwtBearerSecurityRequirement : OpenApiSecurityRequirement
    {
        /// <summary>
        ///     Open API 3.0 JWT Bearer Authorization.
        /// </summary>
        /// <remarks><see href="https://github.com/OAI/OpenAPI-Specification/blob/master/versions/3.0.0.md#jwt-bearer-sample" />.</remarks>
        public OpenApiJwtBearerSecurityRequirement()
        {
            Add(
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http, BearerFormat = "JWT", Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = JwtBearerDefaults.AuthenticationScheme
                    }
                },
                new List<string>()); // list of string scopes is required for something like OAuth)
        }
    }
}
