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
        Response.StatusCode = statusCode.HasValue ? statusCode.Value : (int)HttpStatusCode.InternalServerError;

        return View(nameof(PageType.Error), new BaseViewModel());
    }
}