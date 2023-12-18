using FrontendObligationChecker.Models.Cookies;

namespace FrontendObligationChecker.Services.Infrastructure.Interfaces;

public interface ICookieService
{
    void SetCookieAcceptance(bool accept, IRequestCookieCollection cookies, IResponseCookies responseCookies);

    CookieConsentState GetConsentState(IRequestCookieCollection cookies, IResponseCookies responseCookies);
}