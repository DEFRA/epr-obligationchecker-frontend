namespace FrontendObligationChecker.Controllers;

using Microsoft.AspNetCore.Mvc;
using Models.ObligationChecker;
using ViewModels;

[Route(PagePath.Error)]
public class ErrorController : Controller
{
    public ViewResult Error(int? statusCode)
    {
        return View(nameof(PageType.Error), new BaseViewModel() { });
    }
}