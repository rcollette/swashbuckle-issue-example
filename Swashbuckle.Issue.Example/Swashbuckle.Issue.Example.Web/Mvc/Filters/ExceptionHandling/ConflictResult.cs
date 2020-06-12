using Microsoft.AspNetCore.Mvc;

namespace Swashbuckle.Issue.Example.Web.Mvc.Filters.ExceptionHandling
{
    public class ConflictResult : ContentResult
    {
        // TODO we need a defined object type for the response rather than just a message.
        public ConflictResult(string message)
        {
            StatusCode = 409;
            Content = message;
        }
    }
}
