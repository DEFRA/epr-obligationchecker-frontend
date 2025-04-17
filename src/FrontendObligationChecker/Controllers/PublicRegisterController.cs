namespace FrontendObligationChecker.Controllers
{
    using System.Globalization;
    using FrontendObligationChecker.Constants;
    using FrontendObligationChecker.Constants.PublicRegister;
    using FrontendObligationChecker.Models.BlobReader;
    using FrontendObligationChecker.Models.Config;
    using FrontendObligationChecker.Services.PublicRegister;
    using FrontendObligationChecker.ViewModels.PublicRegister;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.PublicRegisterEnabled)]
    [Route(PagePath.PublicRegister)]
    public class PublicRegisterController(
        IBlobStorageService blobStorageService,
        IOptions<PublicRegisterOptions> publicRegisterOptions) : Controller
    {
        private IBlobStorageService _blobStorageService = blobStorageService;
        private IOptions<PublicRegisterOptions> _publicRegisterOptions = publicRegisterOptions;

        [HttpGet]
        public async Task<IActionResult> Guidance()
        {
            PublicRegisterBlobModel producerBlobModel = await _blobStorageService.GetLatestFilePropertiesAsync(
            _publicRegisterOptions.Value.PublicRegisterBlobContainerName);

            PublicRegisterBlobModel complianceSchemeBlobModel = await _blobStorageService.GetLatestFilePropertiesAsync(
                _publicRegisterOptions.Value.PublicRegisterCsoBlobContainerName);

            string publishedDate = producerBlobModel.PublishedDate.ToString("d MMMM yyyy", CultureInfo.InvariantCulture);
            string lastUpdated = producerBlobModel.LastModified?.ToString("d MMMM yyyy", CultureInfo.InvariantCulture) ?? publishedDate;
            string producersRegisteredFileSize = producerBlobModel.ContentLength?.ToString() ?? "0";
            string producersRegisteredFileType = producerBlobModel.FileType;

            string complianceSchemeRegisteredFileSize = complianceSchemeBlobModel.ContentLength?.ToString() ?? "0";
            string complianceSchemeRegisteredFileType = complianceSchemeBlobModel.FileType;

            var viewModel = new GuidanceViewModel
            {
                PublishedDate = publishedDate,
                LastUpdated = lastUpdated,
                ProducersRegisteredFileSize = producersRegisteredFileSize,
                ProducersRegisteredFileType = producersRegisteredFileType,
                ComplianceSchemesRegisteredFileSize = complianceSchemeRegisteredFileSize,
                ComplianceSchemesRegisteredFileType = complianceSchemeRegisteredFileType
            };

            return View("Guidance", viewModel);
        }
    }
}