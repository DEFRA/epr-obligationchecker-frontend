namespace FrontendObligationChecker.Controllers;

using Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Models.Config;
using Models.Cookies;
using Models.LargeProducerRegister;
using Services.Infrastructure.Interfaces;
using ViewModels;

[Route(PagePath.Cookies)]
public class CookiesController : Controller
{
    private readonly ICookieService _cookieService;
    private readonly EprCookieOptions _eprCookieOptions;
    private readonly AnalyticsOptions _googleAnalyticsOptions;

    public CookiesController(
        ICookieService cookieService,
        IOptions<EprCookieOptions> eprCookieOptions,
        IOptions<AnalyticsOptions> googleAnalyticsOptions)
    {
        _cookieService = cookieService;
        _eprCookieOptions = eprCookieOptions.Value;
        _googleAnalyticsOptions = googleAnalyticsOptions.Value;
    }

    [HttpPost]
    [Route(PagePath.Update)]
    public LocalRedirectResult UpdateAcceptance(string returnUrl, string cookies)
    {
        _cookieService.SetCookieAcceptance(cookies == CookieAcceptance.Accept, Request.Cookies, Response.Cookies);

        return LocalRedirect(returnUrl);
    }

    [HttpPost]
    [Route(PagePath.Confirm)]
    public LocalRedirectResult ConfirmAcceptance(string returnUrl)
    {
        return LocalRedirect(returnUrl);
    }

    public async Task<IActionResult> Detail(string returnUrl)
    {
        var cookieConsentState = _cookieService.GetConsentState(Request.Cookies, Response.Cookies);

        if (!Url.IsLocalUrl(returnUrl))
        {
            returnUrl = Url.HomePath();
        }

        var cookieViewModel = new CookieDetailViewModel
        {
            CurrentPage = returnUrl,
            BackLinkToDisplay = Url.Content(returnUrl),
            SessionCookieName = _eprCookieOptions.SessionCookieName,
            CookiePolicyCookieName = _eprCookieOptions.CookiePolicyCookieName,
            AntiForgeryCookieName = _eprCookieOptions.AntiForgeryCookieName,
            GoogleAnalyticsDefaultCookieName = _googleAnalyticsOptions.DefaultCookieName,
            GoogleAnalyticsAdditionalCookieName = _googleAnalyticsOptions.AdditionalCookieName,
            CookiesAccepted = cookieConsentState.CookiesAccepted,
            ReturnUrl = $"~{Request.Path}{HttpContext.Request.QueryString}",
            ShowAcknowledgement = cookieConsentState.CookieAcknowledgementRequired

        };

        return View(cookieViewModel);
    }
}