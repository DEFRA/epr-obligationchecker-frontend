namespace FrontendObligationChecker.Controllers
{
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using FrontendObligationChecker.Constants;
    using FrontendObligationChecker.Constants.PublicRegister;
    using FrontendObligationChecker.Exceptions;
    using FrontendObligationChecker.Models.BlobReader;
    using FrontendObligationChecker.Models.Config;
    using FrontendObligationChecker.Services.PublicRegister;
    using FrontendObligationChecker.Services.PublicRegister.Interfaces;
    using FrontendObligationChecker.ViewModels.PublicRegister;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.PublicRegisterEnabled)]
    [Route(PagePath.PublicRegister)]
    public class PublicRegisterController(
        IBlobStorageService blobStorageService,
        IOptions<ExternalUrlsOptions> urlOptions,
        IOptions<PublicRegisterOptions> publicRegisterOptions,
        IFeatureFlagService featureFlagService) : Controller
    {
        private readonly IBlobStorageService _blobStorageService = blobStorageService;
        private readonly PublicRegisterOptions _options = publicRegisterOptions.Value;
        private readonly ExternalUrlsOptions _urlOptions = urlOptions.Value;
        private readonly IFeatureFlagService _featureFlagService = featureFlagService;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var isComplianceSchemesRegisterEnabled = await _featureFlagService.IsComplianceSchemesRegisterEnabledAsync();
            var isEnforcementActionsSectionEnabled = await _featureFlagService.IsEnforcementActionsSectionEnabledAsync();

            var producerBlobModel = await _blobStorageService
                .GetLatestFilePropertiesAsync(_options.PublicRegisterBlobContainerName);

            var publishedDate = FormatDate(producerBlobModel.PublishedDate);
            var lastUpdated = FormatDate(producerBlobModel.LastModified) ?? publishedDate;

            var viewModel = new GuidanceViewModel
            {
                DefraUrl = _urlOptions.DefraUrl,
                PublishedDate = publishedDate,
                LastUpdated = lastUpdated,
                ProducerRegisteredFile = MapToFileViewModel(producerBlobModel, publishedDate, lastUpdated)
            };

            if (isComplianceSchemesRegisterEnabled)
            {
                var complianceBlobModel = await _blobStorageService
                    .GetLatestFilePropertiesAsync(_options.PublicRegisterCsoBlobContainerName);

                viewModel.ComplianceSchemeRegisteredFile = MapToFileViewModel(complianceBlobModel, publishedDate, lastUpdated);
            }

            if (isEnforcementActionsSectionEnabled)
            {
                 viewModel.EnforcementActionFiles = await _blobStorageService.GetEnforcementActionFiles();
            }

            return View("Guidance", viewModel);
        }

        [HttpGet(PagePath.Enforce)]
        [Produces("text/csv")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ExcludeFromCodeCoverage]
        public async Task<IActionResult> File(string agency)
        {
            if (string.IsNullOrEmpty(agency))
            {
                return RedirectToAction(nameof(PagePath.FileNotDownloaded));
            }

            try
            {
                var latestFile = await _blobStorageService.GetEnforcementActionFileByAgency(agency);

                if (latestFile == null || latestFile.FileContents == null)
                {
                    return RedirectToAction(nameof(PagePath.FileNotDownloaded));
                }

                return File(latestFile.FileContents, "text/csv", latestFile.FileName);
            }
            catch (LargeProducerRegisterServiceException ex)
            {
                return RedirectToAction(nameof(PagePath.FileNotDownloaded));
            }
        }

        [HttpGet(PagePath.Report)]
        [Produces("text/csv")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> File(string fileName, string type)
        {
            try
            {
                var containerName = type == _options.PublicRegisterBlobContainerName ? _options.PublicRegisterBlobContainerName : _options.PublicRegisterCsoBlobContainerName;
                var fileModel = await _blobStorageService.GetLatestFileAsync(containerName);
                if (string.IsNullOrEmpty(fileModel.FileName))
                {
                    return RedirectToAction(nameof(PagePath.FileNotDownloaded));
                }

                return File(fileModel.FileContent, "text/csv", fileModel.FileName);
            }
            catch (PublicRegisterServiceException ex)
            {
                return RedirectToAction(nameof(PagePath.FileNotDownloaded));
            }
        }

        [HttpGet(PagePath.FileNotDownloaded)]
        public async Task<IActionResult> FileNotDownloaded()
        {
            return View("GuidanceError", new PublicRegisterErrorViewModel());
        }

        private static string FormatDate(DateTime? date) =>
            date?.ToString("d MMMM yyyy", CultureInfo.InvariantCulture);

        private static PublicRegisterFileViewModel MapToFileViewModel(PublicRegisterBlobModel blobModel, string publishedDate, string lastUpdated)
        {
            return new PublicRegisterFileViewModel
            {
                DatePublished = publishedDate,
                DateLastModified = lastUpdated,
                FileName = blobModel.Name,
                FileSize = blobModel.ContentLength?.ToString() ?? "0",
                FileType = blobModel.FileType
            };
        }
    }
}