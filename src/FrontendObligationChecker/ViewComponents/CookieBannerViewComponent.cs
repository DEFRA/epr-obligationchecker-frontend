using FrontendObligationChecker.Services.Infrastructure.Interfaces;
using FrontendObligationChecker.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FrontendObligationChecker.ViewComponents;

public class CookieBannerViewComponent : ViewComponent
{
    private readonly ICookieService _cookieService;

    public CookieBannerViewComponent(ICookieService cookieService)
    {
        _cookieService = cookieService;
    }

    public IViewComponentResult Invoke()
    {
        var cookieConsentState = _cookieService.GetConsentState(Request.Cookies, HttpContext.Response.Cookies);
        var onCookiePage = ViewContext.RouteData.Values["controller"].ToString() == "Cookies";
        var cookieBannerModel = new CookieBannerModel
        {
            ShowBanner = !onCookiePage && !cookieConsentState.CookieAcknowledgementRequired && !cookieConsentState.CookieExists,
            ShowAcknowledgement = !onCookiePage && cookieConsentState.CookieAcknowledgementRequired,
            AcceptAnalytics = cookieConsentState.CookiesAccepted,
            ReturnUrl = $"~{Request.Path.Value}{Request.QueryString}"
        };

        return View(cookieBannerModel);
    }
}