using Freelance.Api.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Freelance.Api.Models;

/// <summary>
/// Фильтр для операций API для обработки исключений.
/// </summary>
public class ApiErrorResponseFilter : IActionFilter, IOrderedFilter
{
    public int Order => int.MaxValue - 10;

    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is ApiException exception)
        {
            if (!string.IsNullOrEmpty(exception.Message))
            {
                context.ModelState.AddModelError(exception.ModelField ?? string.Empty, exception.Message);
            }
            var errDict = context.ModelState.Where(i => i.Value?.Errors.Count > 0).Select(i => new
            {
                Field = i.Key,
                Errors = i.Value!.Errors.Select(i => i.ErrorMessage).ToArray()
            }).ToDictionary(i => i.Field, i => i.Errors);

            if (context.Exception is ApiNotFoundException)
            {
                context.Result = new NotFoundObjectResult(new ApiErrorResponse()
                {
                    Errors = errDict
                });
            }
            else if (context.Exception is ApiForbidException)
            {
                context.Result = new ForbidResult();
            }
            else
            {
                context.Result = new BadRequestObjectResult(new ApiErrorResponse()
                {
                    Errors = errDict
                });
            }

            context.ExceptionHandled = true;
        }
    }
}
