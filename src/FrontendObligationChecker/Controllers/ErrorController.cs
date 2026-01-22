namespace FrontendObligationChecker.Controllers;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.ObligationChecker;
using ViewModels;

[Route(PagePath.Error)]
public class ErrorController(ILogger<ErrorController> logger) : Controller
{
    public ViewResult Error(int? statusCode)
    {
        Response.StatusCode = statusCode ?? StatusCodes.Status500InternalServerError;

        var exception = HttpContext.Features.Get<IExceptionHandlerFeature>();

        if (exception != null)
        {
            logger.LogWarning(exception.Error, "Unhandled exception caught for request path: {RequestPath}", exception.Path);
        }

        if (statusCode == StatusCodes.Status404NotFound)
        {
            var originalRequest = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            logger.LogWarning("404 NotFound path: {RequestPath}{RequestQuery}", originalRequest.OriginalPath, originalRequest.OriginalQueryString ?? string.Empty);
        }

        return View(nameof(PageType.Error), new BaseViewModel());
    }
}