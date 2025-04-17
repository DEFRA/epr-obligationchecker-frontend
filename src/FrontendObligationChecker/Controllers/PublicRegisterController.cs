namespace FrontendObligationChecker.Controllers
{
    using FrontendObligationChecker.Constants;
    using FrontendObligationChecker.Constants.PublicRegister;
    using FrontendObligationChecker.Services.PublicRegister;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.PublicRegisterEnabled)]
    [Route(PagePath.PublicRegister)]
    public class PublicRegisterController : Controller
    {
        private IBlobStorageService blobStorageService;

        public PublicRegisterController(IBlobStorageService blobStorageService)
        {
            this.blobStorageService = blobStorageService;
        }

        [HttpGet]
        public async Task<IActionResult> Guidance()
        {
            var viewModel = await blobStorageService.GetGuidanceViewModelAsync();

            return View("Guidance", viewModel);
        }
    }
}