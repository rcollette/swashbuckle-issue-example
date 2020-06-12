using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using ZNetCS.AspNetCore.Authentication.Basic;

namespace Swashbuckle.Issue.Example.Web.Swagger
{
    public class OpenApiBasicAuthSecurityRequirement : OpenApiSecurityRequirement
    {
        /// <summary>
        ///     Open API 3.0 Basic Authentication.
        /// </summary>
        /// <remarks><see href="https://github.com/OAI/OpenAPI-Specification/blob/master/versions/3.0.0.md#jwt-bearer-sample" />.</remarks>
        public OpenApiBasicAuthSecurityRequirement()
        {
            Add(
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Scheme = "basic",
                    // A requirement references a scheme
                    // This reference refers to the requirment defined by
                    // c.AddSecurityDefinition(
                    //    BasicAuthenticationDefaults.AuthenticationScheme,
                    //    new OpenApiBasicAuthSecurityScheme());
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = BasicAuthenticationDefaults.AuthenticationScheme
                    }
                },
                new List<string>()); // list of string scopes is required for something like OAuth
        }
    }
}
