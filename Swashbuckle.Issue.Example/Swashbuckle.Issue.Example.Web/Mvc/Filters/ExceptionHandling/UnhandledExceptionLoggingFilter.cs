using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Swashbuckle.Issue.Example.Web.Mvc.Filters.ExceptionHandling
{
    /// <summary>
    ///     Logs all Exceptions that are unhandled at an ERROR level.
    /// </summary>
    public class UnhandledExceptionLoggingFilter : IExceptionFilter
    {
        private readonly ILogger _logger;

        // ReSharper disable once SuggestBaseTypeForParameter
        public UnhandledExceptionLoggingFilter(
            ILogger<UnhandledExceptionLoggingFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(new EventId(-1, "Unhandled Exception"), context.Exception, "Unhandled Exception");
        }
    }
}
