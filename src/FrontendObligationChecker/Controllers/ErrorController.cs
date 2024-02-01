namespace FrontendObligationChecker.Controllers;

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Models.ObligationChecker;
using ViewModels;

[Route(PagePath.Error)]
public class ErrorController : Controller
{
    public ViewResult Error(int? statusCode)
    {
        var errorView = statusCode == (int?)HttpStatusCode.NotFound ? nameof(PageType.PageNotFound) : nameof(PageType.Error);
        return View(errorView, new BaseViewModel());
    }
}