namespace FrontendObligationChecker.Controllers;

using Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Models.Config;
using Models.LargeProducerRegister;
using ViewModels;

[Route(PagePath.Privacy)]
public class PrivacyController : Controller
{
    private readonly ExternalUrlsOptions _urlOptions;
    private readonly EmailAddressOptions _emailOptions;
    private readonly SiteDateOptions _siteDateOptions;

    public PrivacyController(
        IOptions<ExternalUrlsOptions> urlOptions,
        IOptions<EmailAddressOptions> emailOptions,
        IOptions<SiteDateOptions> siteDateOptions)
    {
        _urlOptions = urlOptions?.Value;
        _emailOptions = emailOptions?.Value;
        _siteDateOptions = siteDateOptions?.Value;
    }

    [HttpGet]
    public async Task<IActionResult> Detail(string returnUrl)
    {
        if (!Url.IsLocalUrl(returnUrl))
        {
            returnUrl = Url.HomePath();
        }

        var model = new PrivacyViewModel
        {
            CurrentPage = returnUrl,
            BackLinkToDisplay = Url.Content(returnUrl),
            DataProtectionPublicRegisterUrl = _urlOptions.PrivacyDataProtectionPublicRegister,
            WebBrowserUrl = _urlOptions.PrivacyWebBrowser,
            GoogleAnalyticsUrl = _urlOptions.PrivacyGoogleAnalytics,
            EuropeanEconomicAreaUrl = _urlOptions.PrivacyEuropeanEconomicArea,
            DefrasPersonalInformationCharterUrl = _urlOptions.PrivacyDefrasPersonalInformationCharter,
            InformationCommissionerUrl = _urlOptions.PrivacyInformationCommissioner,
            FindOutAboutCallChargesUrl = _urlOptions.PrivacyFindOutAboutCallCharges,
            ScottishEnvironmentalProtectionAgencyUrl = _urlOptions.PrivacyScottishEnvironmentalProtectionAgency,
            NationalResourcesWalesUrl = _urlOptions.PrivacyNationalResourcesWales,
            NorthernIrelandEnvironmentAgencyUrl = _urlOptions.PrivacyNorthernIrelandEnvironmentAgency,
            EnvironmentAgencyUrl = _urlOptions.PrivacyEnvironmentAgency,
            DataProtectionEmail = _emailOptions.DataProtection,
            InformationCommissionerEmail = _emailOptions.InformationCommissioner,
            DefraGroupProtectionOfficerEmail = _emailOptions.DefraGroupProtectionOfficer,
            LastUpdated = _siteDateOptions.PrivacyLastUpdated.ToString(_siteDateOptions.DateFormat)
        };

        return View(model);
    }
}