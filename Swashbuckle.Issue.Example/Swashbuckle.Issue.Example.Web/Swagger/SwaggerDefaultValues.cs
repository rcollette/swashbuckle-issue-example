using System.Linq;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Swashbuckle.Issue.Example.Web.Swagger
{
    /// <summary>
    ///     Represents the Swagger/Swashbuckle operation filter used to document the implicit API version parameter.
    /// </summary>
    /// <remarks>
    ///     This <see cref="IOperationFilter" /> is only required due to bugs in the <see cref="SwaggerGenerator" />.
    ///     Once they are fixed and published, this class can be removed.
    /// </remarks>
    public class SwaggerDefaultValues : IOperationFilter
    {
        /// <summary>
        ///     Applies the filter to the specified operation using the given context.
        /// </summary>
        /// <param name="operation">The operation to apply the filter to.</param>
        /// <param name="context">The current operation filter context.</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                return;
            }

            foreach (OpenApiParameter parameter in operation.Parameters)
            {
                ApiParameterDescription description =
                    context.ApiDescription.ParameterDescriptions.First(p => p.Name == parameter.Name);
                ApiParameterRouteInfo routeInfo = description.RouteInfo;

                if (parameter.Description == null)
                {
                    parameter.Description = description.ModelMetadata?.Description;
                }

                if (routeInfo == null)
                {
                    continue;
                }

                if (parameter.Schema.Default == null && description.DefaultValue != null)
                {
                    parameter.Schema.Default = OpenApiAnyFactory.CreateFor(parameter.Schema, description.DefaultValue);
                }

                parameter.Required |= !routeInfo.IsOptional;
            }
        }
    }
}
