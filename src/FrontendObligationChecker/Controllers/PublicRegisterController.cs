namespace FrontendObligationChecker.Controllers
{
    using System.Globalization;
    using FrontendObligationChecker.Constants;
    using FrontendObligationChecker.Constants.PublicRegister;
    using FrontendObligationChecker.Sessions;
    using FrontendObligationChecker.ViewModels.PublicRegister;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.PublicRegisterEnabled)]
    [Route(PagePath.PublicRegister)]
    public class PublicRegisterController : Controller
    {
        private readonly SessionRequestCultureProvider _sessionRequestCultureProvider;

        public PublicRegisterController()
        {
            _sessionRequestCultureProvider = new SessionRequestCultureProvider();
        }

        [HttpGet]
        public async Task<IActionResult> Guidance()
        {
            // This is hard-coded for the sake of displaying the view for story #523624
            var viewModel = new GuidanceViewModel
            {
                ComplianceSchemesRegisteredFileSize = "450",
                LastUpdated = new DateTime(2025, 3, 10, 0, 0, 0, DateTimeKind.Utc).ToString("d MMMM yyyy", CultureInfo.InvariantCulture),
                ProducersRegisteredFileSize = "115",
                PublishedDate = new DateTime(2025, 12, 6, 0, 0, 0, DateTimeKind.Utc).ToString("d MMMM yyyy", CultureInfo.InvariantCulture)
            };

            return View("Guidance", viewModel);
        }
    }
}