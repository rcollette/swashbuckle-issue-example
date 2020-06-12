using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.Issue.Example.Web.Controllers.Info;

namespace Swashbuckle.Issue.Example.Web.Controllers
{
    [ApiVersionNeutral]
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class InfoController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public InfoController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        ///     Return information about the application such as its version.
        /// </summary>
        /// <returns>Environment and assembly info.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ApplicationInfo GetInfo()
        {
            return new ApplicationInfo(
                _webHostEnvironment.EnvironmentName,
                _webHostEnvironment.ApplicationName,
                Assembly.GetEntryAssembly()?.GetName()?.Version?.ToString());
        }
    }
}
