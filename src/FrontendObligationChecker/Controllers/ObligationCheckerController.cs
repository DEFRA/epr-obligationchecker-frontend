namespace FrontendObligationChecker.Controllers;

using Constants;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.FeatureManagement.Mvc;
using Models.ObligationChecker;
using Services.NextFinder;
using Services.PageService.Interfaces;
using ViewModels;

[FeatureGate(FeatureFlags.ObligationCheckerEnabled)]
public class ObligationCheckerController : Controller
{
    private readonly ILogger<ObligationCheckerController> _logger;
    private readonly IPageService _pageService;

    public ObligationCheckerController(
        ILogger<ObligationCheckerController> logger,
        IPageService pageService,
        IConfiguration configuration)
    {
        _logger = logger;
        _logger.LogWarning("HELLO1");
        _pageService = pageService;
    }

    [HttpGet]
    [Route("ObligationChecker/{path}")]
    public async Task<IActionResult> Question(string path)
    {
        _logger.LogWarning("HELLO2");
        var page = await _pageService.GetPageAsync(path);

        if (page == null)
        {
            return NotFound();
        }

        var pageModel = new PageModel(page) { CurrentPage = $"~{Request.Path}{Request.QueryString}" };
        return View(page.View, pageModel);
    }

    [HttpPost]
    [Route("ObligationChecker/{path}", Name = "GetNextPage")]
    public async Task<IActionResult> GetNextPage(string path)
    {
        _logger.LogWarning("HELLO3");
        var page = await _pageService.SetAnswersAndGetPageAsync(path, Request.Form);

        if (page == null)
        {
            _logger.LogWarning("HELLO4");
            return Redirect(PagePath.TypeOfOrganisation);
        }

        if (page.HasError)
        {
            _logger.LogWarning("HELLO5");
            return View(page.View, new PageModel(page));
        }

        _logger.LogWarning("HELLO6");
        var nextPath = Url.RouteUrl("GetNextPage", new { path = PageFinder.GetNextPath(page) });

        _logger.LogWarning("HELLO7 " + nextPath + " - requestScheme = " + Request.Scheme);
        return Redirect(nextPath);
    }
}