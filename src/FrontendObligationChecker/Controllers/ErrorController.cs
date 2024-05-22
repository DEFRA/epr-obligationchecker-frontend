namespace FrontendObligationChecker.Controllers;

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Models.ObligationChecker;
using ViewModels;

/// <summary>
/// Standard style error controller for handling and maging errors
/// created within the system.
/// </summary>
[Route(PagePath.Error)]
public class ErrorController : Controller
{
    /// <summary>
    /// Default action for handling all errors produced by the system
    /// Returns either Forbidden or Error page.
    /// </summary>
    /// <param name="statusCode">The error status code that occurred.</param>
    /// <returns>Either a forbidden or error page.</returns>
    [Route("{statusCode:int}")]
    public ViewResult Index(int? statusCode)
    {
        if (statusCode.HasValue)
        {
            Response.StatusCode = statusCode.Value;
        }

        if (statusCode == (int)HttpStatusCode.NotFound)
        {
            return View(nameof(PageType.PageNotFound), new BaseViewModel());
        }

        return View(nameof(PageType.Error), new BaseViewModel());
    }
}