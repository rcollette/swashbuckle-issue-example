using Microsoft.OpenApi.Models;

// TODO - Add schemes that match the other security requirements
namespace Swashbuckle.Issue.Example.Web.Swagger
{
    public class OpenApiBasicAuthSecurityScheme : OpenApiSecurityScheme
    {
        public OpenApiBasicAuthSecurityScheme()
        {
            Type = SecuritySchemeType.Http;
            Scheme = "basic";
        }
    }
}
