namespace FrontendObligationChecker.Controllers;

using Exceptions;
using Microsoft.AspNetCore.Mvc;
using Models.LargeProducerRegister;
using Services.Caching;
using Services.LargeProducerRegister.Interfaces;
using Sessions;
using ViewModels;
using ViewModels.LargeProducer;

[Route(PagePath.LargeProducerRegister)]
public class LargeProducerRegisterController : Controller
{
    private const string FileNotDownloadedExceptionLog = "Failed to receive file for nation code {NationCode}";

    private readonly ILargeProducerRegisterService _largeProducerRegisterService;
    private readonly ILogger<LargeProducerRegisterController> _logger;
    private readonly ICacheService _cacheService;
    private readonly SessionRequestCultureProvider _sessionRequestCultureProvider;

    public LargeProducerRegisterController(
        ILargeProducerRegisterService largeProducerRegisterService,
        ILogger<LargeProducerRegisterController> logger,
        ICacheService cacheService)
    {
        _largeProducerRegisterService = largeProducerRegisterService;
        _logger = logger;
        _cacheService = cacheService;
        _sessionRequestCultureProvider = new SessionRequestCultureProvider();
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var culture = _sessionRequestCultureProvider.DetermineProviderCultureResult(HttpContext).Result.Cultures.First().ToString();
        if (!_cacheService.GetReportFileSizeCache(culture, out Dictionary<string, string> reportFileSizeMapping))
        {
            reportFileSizeMapping = await _largeProducerRegisterService.GetAllReportFileSizesAsync(culture);
            _cacheService.SetReportFileSizeCache(culture, reportFileSizeMapping);
        }

        var largeProducerViewModel = new LargeProducerRegisterViewModel
        {
            HomeNationFileSizeMapping = reportFileSizeMapping
        };
        return View("LargeProducerRegister", largeProducerViewModel);
    }

    [HttpGet(PagePath.Report + "/{nationCode}")]
    [Produces("text/csv")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> File(string nationCode)
    {
        var culture = _sessionRequestCultureProvider.DetermineProviderCultureResult(HttpContext).Result.Cultures.First().ToString();
        try
        {
            var producerReport = await _largeProducerRegisterService.GetReportAsync(nationCode, culture);
            return File(producerReport.Stream, "text/csv", producerReport.FileName);
        }
        catch (LargeProducerRegisterServiceException ex)
        {
            _logger.LogError(ex, FileNotDownloadedExceptionLog, nationCode);
            return RedirectToAction("FileNotDownloaded", new { nationCode });
        }
    }

    [HttpGet(PagePath.FileNotDownloaded + "/{nationCode}")]
    public async Task<IActionResult> FileNotDownloaded(string nationCode)
    {
        return View("LargeProducerError", new LargeProducerErrorViewModel
        {
            NationCode = nationCode
        });
    }
}