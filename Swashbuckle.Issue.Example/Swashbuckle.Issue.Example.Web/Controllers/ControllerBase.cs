using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Swashbuckle.Issue.Example.Web.Controllers
{
    public class ControllerBase : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        public override ActionResult ValidationProblem()
        {
            IOptions<ApiBehaviorOptions> options =
                HttpContext.RequestServices.GetRequiredService<IOptions<ApiBehaviorOptions>>();
            return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }

        protected ActionResult<T> OkOrNotFound<T>(T val) where T : class?
        {
            if (val == null)
            {
                return NotFound();
            }

            return val;
        }

        protected ActionResult KeyMismatchBadRequest()
        {
            ModelState.TryAddModelError(
                "key-mismatch",
                "The Id passed in the path and in the body do not match.");
            //The following ensures that the
            return ValidationProblem();
        }
    }
}
