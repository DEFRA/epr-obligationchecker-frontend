namespace FrontendObligationChecker.Controllers;

using ByteSizeLib;
using Exceptions;
using FrontendObligationChecker.Constants;
using Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement.Mvc;
using Models.Config;
using Models.LargeProducerRegister;
using Services.LargeProducerRegister.Interfaces;
using Services.PublicRegister;
using Sessions;
using ViewModels;
using ViewModels.LargeProducer;

[FeatureGate(FeatureFlags.LargeProducerRegisterEnabled)]
[Route(PagePath.LargeProducerRegister)]
public class LargeProducerRegisterController : Controller
{
    private const string FileNotDownloadedExceptionLog = "Failed to receive file for nation code {NationCode}";
    private const int RegisterOfProducersStartYear = 2025;

    private readonly ILargeProducerRegisterService _largeProducerRegisterService;
    private readonly IBlobStorageService _blobStorageService;
    private readonly ILogger<LargeProducerRegisterController> _logger;
    private readonly SessionRequestCultureProvider _sessionRequestCultureProvider;
    private readonly PublicRegisterOptions _publicRegisterOptions;

    public LargeProducerRegisterController(
        ILargeProducerRegisterService largeProducerRegisterService,
        IBlobStorageService blobStorageService,
        IOptions<PublicRegisterOptions> publicRegisterOptions,
        ILogger<LargeProducerRegisterController> logger)
    {
        _largeProducerRegisterService = largeProducerRegisterService;
        _blobStorageService = blobStorageService;
        _publicRegisterOptions = publicRegisterOptions.Value;
        _logger = logger;
        _sessionRequestCultureProvider = new SessionRequestCultureProvider();
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var culture = _sessionRequestCultureProvider.DetermineProviderCultureResult(HttpContext).Result.Cultures[0].ToString();

        var latestFiles = await _largeProducerRegisterService.GetLatestAllNationsFileInfoAsync(culture);

        var listOfLargeProducers = GetListOfLargeProducers(latestFiles);

        var registerOfProducers = await GetRegisterOfProducersAsync();

        var largeProducerViewModel = new LargeProducerRegisterViewModel
        {
            ListOfLargeProducers = listOfLargeProducers,
            RegisterOfProducers = registerOfProducers
        };

        return View("LargeProducerRegister", largeProducerViewModel);
    }

    private static IEnumerable<LargeProducerFileInfoViewModel> GetListOfLargeProducers(
        IEnumerable<LargeProducerFileInfoViewModel> latestFiles)
    {
        return latestFiles
            .Where(x => x.ReportingYear < RegisterOfProducersStartYear)
            .Select(x =>
            {
                x.DownloadUrl = $"/large-producers/report?reportingYear={x.ReportingYear}";
                return x;
            });
    }

    private async Task<IEnumerable<LargeProducerFileInfoViewModel>> GetRegisterOfProducersAsync()
    {
        var containerName = _publicRegisterOptions.PublicRegisterBlobContainerName;

        int currentYear = string.IsNullOrWhiteSpace(_publicRegisterOptions.CurrentYear)
            ? DateTime.UtcNow.Year
            : int.Parse(_publicRegisterOptions.CurrentYear);

        var folderPrefixes = Enumerable
            .Range(RegisterOfProducersStartYear, currentYear - RegisterOfProducersStartYear)
            .Select(y => y.ToString())
            .ToList();

        var blobs = await _blobStorageService.GetLatestFilePropertiesAsync(containerName, folderPrefixes);

        return blobs
            .OrderByDescending(kvp => kvp.Key)
            .Select(kvp => new LargeProducerFileInfoViewModel
            {
                ReportingYear = int.Parse(kvp.Key),
                DateCreated = kvp.Value.LastModified ?? DateTime.UtcNow,
                DisplayFileSize = long.TryParse(kvp.Value.ContentLength, out var bytes)
                    ? FileSizeFormatterHelper.ConvertByteSizeToString(ByteSize.FromBytes(bytes))
                    : kvp.Value.ContentLength ?? "0",
                DownloadUrl = $"/public-register/report?fileName={kvp.Value.Name}&type={containerName}"
            });
    }

    [HttpGet(PagePath.Report)]
    [Produces("text/csv")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> File(int? reportingYear)
    {
        if (!reportingYear.HasValue)
        {
            return RedirectToAction("Get");
        }

        var culture = _sessionRequestCultureProvider.DetermineProviderCultureResult(HttpContext).Result.Cultures[0].ToString();

        try
        {
            var latestFile = await _largeProducerRegisterService.GetLatestAllNationsFileAsync(reportingYear.Value, culture);

            if (latestFile == null)
            {
                _logger.LogError(FileNotDownloadedExceptionLog, HomeNation.All);
                return RedirectToAction("FileNotDownloaded");
            }

            return File(latestFile.FileContents, "text/csv", latestFile.FileName);
        }
        catch (LargeProducerRegisterServiceException ex)
        {
            _logger.LogError(ex, FileNotDownloadedExceptionLog, HomeNation.All);
            return RedirectToAction("FileNotDownloaded");
        }
    }

    [HttpGet(PagePath.FileNotDownloaded)]
    public async Task<IActionResult> FileNotDownloaded()
    {
        return View("LargeProducerError", new LargeProducerErrorViewModel());
    }
}