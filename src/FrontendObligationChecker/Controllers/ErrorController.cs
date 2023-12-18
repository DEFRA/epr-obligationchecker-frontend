using System.Net;

using FrontendObligationChecker.Models.ObligationChecker;
using FrontendObligationChecker.ViewModels;

using Microsoft.AspNetCore.Mvc;

namespace FrontendObligationChecker.Controllers;

public class ErrorController : Controller
{
    [Route("error")]
    public ViewResult Error(int? statusCode)
    {
        var errorView = statusCode == (int?)HttpStatusCode.NotFound ? nameof(PageType.PageNotFound) : nameof(PageType.Error);
        return View(errorView, new BaseViewModel());
    }
}