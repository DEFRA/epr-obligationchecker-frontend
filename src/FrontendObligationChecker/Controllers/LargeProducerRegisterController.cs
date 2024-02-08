namespace FrontendObligationChecker.Controllers;

using Constants;
using Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using Models.LargeProducerRegister;
using Services.LargeProducerRegister.Interfaces;
using ViewModels;
using ViewModels.LargeProducer;

[Route(PagePath.LargeProducerRegister)]
public class LargeProducerRegisterController : Controller
{
    private const string FileNotDownloadedExceptionLog = "Failed to receive file for nation code {NationCode}";
    private const string InvalidArgumentExceptionLog = "Nation code {NationCode} does not exist";
    private const string DisabledArgumentExceptionLog = "Nation code {NationCode} is disabled by feature flag";

    private readonly ILargeProducerRegisterService _largeProducerRegisterService;
    private readonly ILogger<LargeProducerRegisterController> _logger;
    private readonly IFeatureManager _featureManager;

    public LargeProducerRegisterController(
        ILargeProducerRegisterService largeProducerRegisterService,
        ILogger<LargeProducerRegisterController> logger,
        IFeatureManager featureManager)
    {
        _largeProducerRegisterService = largeProducerRegisterService;
        _logger = logger;
        _featureManager = featureManager;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return View("LargeProducerRegister", new BaseViewModel());
    }

    [HttpGet(PagePath.Report + "/{nationCode}")]
    [Produces("text/csv")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> File(string nationCode)
    {
        try
        {
            if (!(await IsFeatureEnabled(nationCode)))
            {
                throw new ArgumentException(DisabledArgumentExceptionLog);
            }

            var producerReport = await _largeProducerRegisterService.GetReportAsync(nationCode);
            return File(producerReport.Stream, "text/csv", producerReport.FileName);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, InvalidArgumentExceptionLog, nationCode);
            return NotFound();
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
        try
        {
            if (!(await IsFeatureEnabled(nationCode)))
            {
                throw new ArgumentException(DisabledArgumentExceptionLog);
            }
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, InvalidArgumentExceptionLog, nationCode);
            return NotFound();
        }

        return View("LargeProducerError", new LargeProducerErrorViewModel
        {
            NationCode = nationCode
        });
    }

    private async Task<bool> IsFeatureEnabled(string nationCode)
    {
        const string nationCodeNotFound = "Nation code not found";

        return nationCode switch
        {
            HomeNation.All => await _featureManager.IsEnabledAsync(FeatureFlags.AllNationsDownloadLink),
            HomeNation.England => await _featureManager.IsEnabledAsync(FeatureFlags.EnglandNationDownloadLink),
            HomeNation.Scotland => await _featureManager.IsEnabledAsync(FeatureFlags.ScotlandNationDownloadLink),
            HomeNation.Wales => await _featureManager.IsEnabledAsync(FeatureFlags.WalesNationDownloadLink),
            HomeNation.NorthernIreland => await _featureManager.IsEnabledAsync(FeatureFlags.NorthernIrelandNationDownloadLink),
            _ => throw new ArgumentException(nationCodeNotFound)
        };
    }
}