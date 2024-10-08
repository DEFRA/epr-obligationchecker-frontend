﻿namespace FrontendObligationChecker.Controllers;

using Exceptions;
using FrontendObligationChecker.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using Models.LargeProducerRegister;
using Services.LargeProducerRegister.Interfaces;
using Sessions;
using ViewModels;
using ViewModels.LargeProducer;

[FeatureGate(FeatureFlags.LargeProducerRegisterEnabled)]
[Route(PagePath.LargeProducerRegister)]
public class LargeProducerRegisterController : Controller
{
    private const string FileNotDownloadedExceptionLog = "Failed to receive file for nation code {NationCode}";

    private readonly ILargeProducerRegisterService _largeProducerRegisterService;
    private readonly ILogger<LargeProducerRegisterController> _logger;
    private readonly SessionRequestCultureProvider _sessionRequestCultureProvider;

    public LargeProducerRegisterController(
        ILargeProducerRegisterService largeProducerRegisterService,
        ILogger<LargeProducerRegisterController> logger)
    {
        _largeProducerRegisterService = largeProducerRegisterService;
        _logger = logger;
        _sessionRequestCultureProvider = new SessionRequestCultureProvider();
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var culture = _sessionRequestCultureProvider.DetermineProviderCultureResult(HttpContext).Result.Cultures[0].ToString();

        var latestFiles = await _largeProducerRegisterService.GetLatestAllNationsFileInfoAsync(culture);

        var largeProducerViewModel = new LargeProducerRegisterViewModel { LatestAllNationsFiles = latestFiles };

        return View("LargeProducerRegister", largeProducerViewModel);
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