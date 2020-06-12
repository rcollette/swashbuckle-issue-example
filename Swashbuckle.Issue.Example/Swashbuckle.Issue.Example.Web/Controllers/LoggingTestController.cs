using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Swashbuckle.Issue.Example.Web.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class LoggingTestController
    {
        private readonly ILogger<LoggingTestController> _logger;

        public LoggingTestController(ILogger<LoggingTestController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<string> Log()
        {
            // To log objects as a named property, the object must be assigned to a variable.
            try
            {
                throw new Exception("critical test exception");
            }
            catch (Exception ex)
            {
                var metaData = new { name = "value" };
                _logger.LogCritical(ex, "Critical test message. {@metaData}", metaData);
            }

            try
            {
                throw new Exception("error test exception");
            }
            catch (Exception ex)
            {
                var metaData = new { name = "value 2" };
                _logger.LogError(ex, "Error test message. {@metaData}", metaData);
            }

            var metaWarning = new { propertyName = "property value" };
            _logger.LogWarning("Warning message. {@metaWarning}", metaWarning);
            var metaInformation = new { my = "data" };
            _logger.LogInformation("Information Message. {@metaInformation}", metaInformation);
            var metaDebug = new { some = "thing" };
            _logger.LogDebug("Debug message. {@metaDebug}", metaDebug);
            var metaTrace = new { another = "thing" };
            _logger.LogTrace("Trace message. {@metaTrace}", metaTrace);
            return "done";
        }
    }
}
