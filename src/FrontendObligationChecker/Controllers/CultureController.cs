using Microsoft.AspNetCore.Mvc;

namespace FrontendObligationChecker.Controllers;
public class CultureController : Controller
{
    [HttpGet]
    [Route("culture")]
    public LocalRedirectResult UpdateCulture(string culture, string returnUrl)
    {
        var urlWithCulture = $"{returnUrl}?culture={culture}";

        return LocalRedirect(urlWithCulture);
    }
}