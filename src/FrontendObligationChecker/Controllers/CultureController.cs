namespace FrontendObligationChecker.Controllers;

using Microsoft.AspNetCore.Mvc;
using Models.LargeProducerRegister;

[Route(PagePath.Culture)]
public class CultureController : Controller
{
    [HttpGet]
    public LocalRedirectResult UpdateCulture(string culture, string returnUrl)
    {
        var urlWithCulture = $"{returnUrl}?culture={culture}";

        return LocalRedirect(urlWithCulture);
    }
}