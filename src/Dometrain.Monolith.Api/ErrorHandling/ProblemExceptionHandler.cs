using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

namespace Dometrain.Monolith.Api.ErrorHandling;

public class ProblemExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;

    public ProblemExceptionHandler(IProblemDetailsService problemDetailsService)
    {
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ValidationException validationException)
        {
            return false;
        }
        
        httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails =
            {
                Title = "A problem occurred",
                Detail = string.Join(", ", validationException.Errors),
                Type = nameof(ValidationException)
            },
            Exception = exception
        });

    }
}
