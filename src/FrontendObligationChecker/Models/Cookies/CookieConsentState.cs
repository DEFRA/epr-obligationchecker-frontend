namespace FrontendObligationChecker.Models.Cookies;

public class CookieConsentState
{
    public bool CookieExists { get; set; }

    public bool CookieAcknowledgementRequired { get; set; }

    public bool CookiesAccepted { get; set; }
}