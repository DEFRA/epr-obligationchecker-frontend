namespace FrontendObligationChecker.Controllers
{
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
        IOptions<EmailAddressOptions> emailAddressOptions,
        IOptions<PublicRegisterOptions> publicRegisterOptions,
        IFeatureFlagService featureFlagService) : Controller
    {
        private readonly IBlobStorageService _blobStorageService = blobStorageService;
        private readonly PublicRegisterOptions _options = publicRegisterOptions.Value;
        private readonly ExternalUrlsOptions _urlOptions = urlOptions.Value;
        private readonly EmailAddressOptions _emailAddressOptions = emailAddressOptions.Value;
        private readonly IFeatureFlagService _featureFlagService = featureFlagService;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var (isComplianceSchemesRegisterEnabled, isEnforcementActionsSectionEnabled) = await GetFeatureFlagsAsync();

            var producerBlobModel = await _blobStorageService
                .GetLatestFilePropertiesAsync(_options.PublicRegisterBlobContainerName);

            var publishedDate = FormatDate(producerBlobModel.PublishedDate);
            var lastUpdated = FormatDate(producerBlobModel.LastModified) ?? publishedDate;

            var viewModel = new GuidanceViewModel
            {
                DefraUrl = _urlOptions.DefraUrl,
                DefraHelplineEmail = _emailAddressOptions.DefraHelpline,
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
                var enforcementActionFiles = await _blobStorageService.GetEnforcementActionFiles();
                viewModel.EnforcementActionFiles = enforcementActionFiles;

                viewModel.EnglishEnforcementActionFile = GetEnforcementActionFileViewModel(enforcementActionFiles, EnforcementAgency.England);
                viewModel.WelshEnforcementActionFile = GetEnforcementActionFileViewModel(enforcementActionFiles, EnforcementAgency.Wales);
                viewModel.NortherIrishEnforcementActionFile = GetEnforcementActionFileViewModel(enforcementActionFiles, EnforcementAgency.NorthernIreland);

                viewModel.ScottishEnforcementActionFileUrl = _urlOptions.PublicRegisterScottishProtectionAgency;
            }

            return View("Guidance", viewModel);
        }

        [HttpGet(PagePath.Enforce)]
        [Produces("text/csv")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> File(string agency)
        {
            if (string.IsNullOrWhiteSpace(agency))
            {
                return RedirectToAction(nameof(PagePath.FileNotDownloaded));
            }

            try
            {
                var latestFile = await _blobStorageService.GetEnforcementActionFileByAgency(agency);

                if (latestFile is null || latestFile.FileContents is null)
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

        private static EnforcementActionFileViewModel? GetEnforcementActionFileViewModel(IEnumerable<EnforcementActionFileViewModel> files, string agency)
        {
            var file = files.FirstOrDefault(x => x.FileName.Contains($"_{agency}"));

            if (file is not null)
            {
                file.FileDownloadUrl = $"/public-register/enforce?&agency={agency}";
            }

            return file;
        }

        private async Task<(bool ComplianceEnabled, bool EnforcementEnabled)> GetFeatureFlagsAsync()
        {
            var compliance = await _featureFlagService.IsComplianceSchemesRegisterEnabledAsync();
            var enforcement = await _featureFlagService.IsEnforcementActionsSectionEnabledAsync();
            return (compliance, enforcement);
        }
    }
}