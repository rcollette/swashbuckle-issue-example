using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.Issue.Example.Repository;

namespace Swashbuckle.Issue.Example.Web.Mvc.Filters.ExceptionHandling
{
    public class ExceptionToResultFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            HandleException(context);
        }

        private static void HandleException(ExceptionContext context)
        {
            Exception exception = context.Exception;
            switch (exception)
            {
                case UniqueConstraintException _:
                    SetConflictResult(context, exception);
                    break;
                case DbUpdateConcurrencyException _:
                    SetConcurrencyResult(context, exception);
                    break;
                case ArgumentException ex:
                    SetBadRequestResult(context, ex);
                    break;
            }
        }

        private static void SetConcurrencyResult(ExceptionContext context, Exception exception)
        {
            context.Result = new ConflictResult("An update has occurred since fetching.");
            context.ModelState.TryAddModelError("conflict", exception, null);
            context.ExceptionHandled = true;
        }

        private static void SetConflictResult(ExceptionContext context, Exception ex)
        {
            context.Result = new ConflictResult(ex.Message);
            context.ExceptionHandled = true;
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        private static void SetBadRequestResult(ExceptionContext context, ArgumentException ex)
        {
            // We do not want to return a key that we get tied to at this point.  We should be using Model validation.
            context.Result = new BadRequestObjectResult(ex.Message);
            context.ExceptionHandled = true;
        }
    }
}
