namespace FrontendObligationChecker.Controllers;

using Constants;
using Microsoft.AspNetCore.Mvc;
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
        _pageService = pageService;
    }

    [HttpGet]
    public async Task<IActionResult> Question(string path)
    {
        var page = await _pageService.GetPageAsync(path);

        if (page == null)
        {
            return NotFound();
        }

        var pageModel = new PageModel(page) { CurrentPage = $"~{Request.Path}{Request.QueryString}" };
        return View(page.View, pageModel);
    }

    [HttpPost]
    public async Task<IActionResult> GetNextPage(string path)
    {
        var page = await _pageService.SetAnswersAndGetPageAsync(path, Request.Form);

        if (page == null)
        {
            return Redirect(PagePath.TypeOfOrganisation);
        }

        if (page.HasError)
        {
            return View(page.View, new PageModel(page));
        }

        var nextPath = Url.RouteUrl("ObligationChecker", new { path = PageFinder.GetNextPath(page) });

        return Redirect(nextPath + Request.QueryString);
    }
}