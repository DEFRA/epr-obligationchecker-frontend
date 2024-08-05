using FrontendObligationChecker.Models.Config;
using FrontendObligationChecker.Models.Cookies;
using FrontendObligationChecker.Services.Infrastructure.Interfaces;
using FrontendObligationChecker.Services.Wrappers.Interfaces;

using Microsoft.Extensions.Options;

namespace FrontendObligationChecker.Services.Infrastructure;

public class CookieService : ICookieService
{
    private readonly ILogger<CookieService> _logger;
    private readonly IDateTimeWrapper _dateTimeWrapper;
    private readonly EprCookieOptions _eprCookieOptions;
    private readonly AnalyticsOptions _googleAnalyticsOptions;

    public CookieService(
        ILogger<CookieService> logger,
        IDateTimeWrapper dateTimeWrapper,
        IOptions<EprCookieOptions> eprCookieOptions,
        IOptions<AnalyticsOptions> googleAnalyticsOptions)
    {
        _logger = logger;
        _dateTimeWrapper = dateTimeWrapper;
        _eprCookieOptions = eprCookieOptions.Value;
        _googleAnalyticsOptions = googleAnalyticsOptions.Value;
    }

    public void SetCookieAcceptance(bool accept, IRequestCookieCollection cookies, IResponseCookies responseCookies)
    {
        try
        {
            if (!accept)
            {
                var existingCookies = cookies?.Where(c => c.Key.StartsWith(_googleAnalyticsOptions.CookiePrefix)).ToList();

                if (existingCookies != null)
                {
                    foreach (var cookie in existingCookies)
                    {
                        responseCookies.Append(
                            key: cookie.Key,
                            value: cookie.Value,
                            options: new CookieOptions()
                            {
                                Expires = _dateTimeWrapper.UtcNow.AddYears(-1),
                                HttpOnly = true,
                                Secure = true,
                                SameSite = SameSiteMode.Strict,
                            });
                    }
                }
            }

            var cookieName = _eprCookieOptions.CookiePolicyCookieName;
            ArgumentNullException.ThrowIfNull(cookieName);

            responseCookies.Append(
                key: cookieName,
                value: $"{accept.ToString()}|{CookieAcceptance.CookieAck}",
                options: new CookieOptions()
                {
                    Expires = _dateTimeWrapper.UtcNow.AddMonths(CookieAcceptance.CookieDurationInMonths),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cookie acceptance to '{S}'", accept.ToString());
            throw;
        }
    }

    public CookieConsentState GetConsentState(IRequestCookieCollection cookies, IResponseCookies responseCookies)
    {
        var cookieConsentState = new CookieConsentState();

        var cookieName = _eprCookieOptions.CookiePolicyCookieName;
        ArgumentNullException.ThrowIfNull(cookieName);

        var cookie = cookies[cookieName];

        if (!string.IsNullOrWhiteSpace(cookie))
        {
            cookieConsentState.CookieExists = true;
        }
        else
        {
            return cookieConsentState;
        }

        var cookieParts = cookie.Split("|");
        if (cookieParts.Length > 0 && cookieParts[0].Equals(true.ToString()))
        {
            cookieConsentState.CookiesAccepted = true;
        }

        if (cookieParts.Length > 1 && cookieParts[1].Equals(CookieAcceptance.CookieAck))
        {
            cookieConsentState.CookieAcknowledgementRequired = true;
            responseCookies.Append(
                key: _eprCookieOptions.CookiePolicyCookieName,
                value: cookie.Split("|")[0],
                options: new CookieOptions()
                {
                    Expires = _dateTimeWrapper.UtcNow.AddMonths(CookieAcceptance.CookieDurationInMonths),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                });
        }

        return cookieConsentState;
    }
}