namespace FrontendObligationChecker.Controllers;

using Constants;
using Microsoft.AspNetCore.Mvc;
using Models.LargeProducerRegister;

[Route(PagePath.Culture)]
public class CultureController : Controller
{
    [HttpGet]
    public LocalRedirectResult UpdateCulture(string culture, string returnUrl)
    {
        HttpContext.Session.SetString(Language.SessionLanguageKey, culture);
        return LocalRedirect(returnUrl);
    }
}