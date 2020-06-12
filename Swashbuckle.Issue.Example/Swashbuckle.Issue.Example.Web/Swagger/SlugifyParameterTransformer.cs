using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Routing;

namespace Swashbuckle.Issue.Example.Web.Swagger
{
    /// <summary>
    ///     Transforms camel or pascal cased parameters and controller names to lower case dashed format.
    /// </summary>
    /// <remarks>
    ///     See <a href="https://stackoverflow.com/a/53838976/107683">https://stackoverflow.com/a/53838976/107683</a>.
    /// </remarks>
    public class SlugifyParameterTransformer : IOutboundParameterTransformer
    {
        public string? TransformOutbound(object value)
        {
            return value == null ? null : Regex.Replace(value.ToString(), "([a-z])([A-Z])", "$1-$2").ToLower();
        }
    }
}
