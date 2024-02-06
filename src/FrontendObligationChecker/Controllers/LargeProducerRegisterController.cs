namespace FrontendObligationChecker.Controllers;

using Exceptions;
using Microsoft.AspNetCore.Mvc;
using Models.LargeProducerRegister;
using Services.LargeProducerRegister.Interfaces;
using ViewModels;

[Route(PagePath.LargeProducerRegister)]
public class LargeProducerRegisterController : Controller
{
    private const string ErrorLog = "Failed to receive file for nation code {NationCode}";

    private readonly ILargeProducerRegisterService _largeProducerRegisterService;
    private readonly ILogger<LargeProducerRegisterController> _logger;

    public LargeProducerRegisterController(
        ILargeProducerRegisterService largeProducerRegisterService, ILogger<LargeProducerRegisterController> logger)
    {
        _largeProducerRegisterService = largeProducerRegisterService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return View("LargeProducerRegister", new BaseViewModel());
    }

    [HttpGet(PagePath.Report + "/{nationCode}")]
    [Produces("text/csv")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> File(string nationCode)
    {
        try
        {
            var producerReport = await _largeProducerRegisterService.GetReportAsync(nationCode);
            return File(producerReport.Stream, "text/csv", producerReport.FileName);
        }
        catch (LargeProducerRegisterServiceException ex)
        {
            _logger.LogError(ex, ErrorLog, nationCode);
            return RedirectToAction("Error");
        }
    }

    [HttpGet(PagePath.Error)]
    public async Task<IActionResult> Error()
    {
        return View("LargeProducerError", new BaseViewModel());
    }
}