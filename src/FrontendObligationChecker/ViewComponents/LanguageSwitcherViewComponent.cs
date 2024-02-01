using FrontendObligationChecker.ViewModels;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FrontendObligationChecker.ViewComponents;

public class LanguageSwitcherViewComponent : ViewComponent
{
    private readonly IOptions<RequestLocalizationOptions> _localizationOptions;

    public LanguageSwitcherViewComponent(IOptions<RequestLocalizationOptions> localizationOptions)
    {
        _localizationOptions = localizationOptions;
    }

    public IViewComponentResult Invoke()
    {
        var cultureFeature = HttpContext.Features.Get<IRequestCultureFeature>();
        var languageSwitcherModel = new LanguageSwitcherModel
        {
            SupportedCultures = _localizationOptions.Value.SupportedCultures!.ToList(),
            CurrentCulture = cultureFeature!.RequestCulture.Culture,
            ReturnUrl = $"~{Request.Path}"
        };

        return View(languageSwitcherModel);
    }
}