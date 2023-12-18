using FluentAssertions;

using FrontendObligationChecker.Models.ObligationChecker;
using FrontendObligationChecker.Services.NextFinder;
using FrontendObligationChecker.Services.PageService;
using FrontendObligationChecker.UnitTests.Helpers;

namespace FrontendObligationChecker.UnitTests.Models.ObligationChecker.PageModelTests;

[TestClass]
public class SetVisitedPagesTests
{
    private PageService _pageService;

    [TestInitialize]
    public void TestInit()
    {
        _pageService = TestPageService.GetPageService();
    }

    [TestMethod]
    public async Task OnTypeOfOrganisationPage_WhenTypeOfOrganisationIsNotSet_ThenNoneOfOptionsAreSelected()
    {
        _pageService.SetVisitedPages(new VisitedPages() { TypeOfOrganisation = TypeOfOrganisation.NotSet });

        Page page = await _pageService.GetPageAsync(PagePath.TypeOfOrganisation);

        Assert.IsNotNull(page);

        page.Questions.First().Options.Where(o => o.IsSelected is not null).Should().BeEmpty();
    }

    [TestMethod]
    public async Task OnTypeOfOrganisationPage_WhenTypeOfOrganisationIsParentCompany_ThenItsOptionGetsSelected()
    {
        _pageService.SetVisitedPages(new VisitedPages() { TypeOfOrganisation = TypeOfOrganisation.ParentCompany });

        Page page = await _pageService.GetPageAsync(PagePath.TypeOfOrganisation);

        Assert.AreEqual(1, page.Questions.First().Options.Count(o => o.IsSelected == true));

        Assert.AreEqual("parent", page.Questions.First().Options.First(o => o.IsSelected == true).Value);
    }

    [TestMethod]
    public async Task OnTypeOfOrganisationPage_WhenTypeOfOrganisationIsSubsidiary_ThenItsOptionGetsSelected()
    {
        _pageService.SetVisitedPages(new VisitedPages() { TypeOfOrganisation = TypeOfOrganisation.Subsidiary });

        Page page = await _pageService.GetPageAsync(PagePath.TypeOfOrganisation);

        Assert.AreEqual(1, page.Questions.First().Options.Count(o => o.IsSelected == true));

        Assert.AreEqual("subsidiary", page.Questions.First().Options.First(o => o.IsSelected == true).Value);
    }

    [TestMethod]
    public async Task OnTypeOfOrganisationPage_WhenTypeOfOrganisationIsIndividualCompany_ThenItsOptionGetsSelected()
    {
        _pageService.SetVisitedPages(new VisitedPages() { TypeOfOrganisation = TypeOfOrganisation.IndividualCompany });

        Page page = await _pageService.GetPageAsync(PagePath.TypeOfOrganisation);

        Assert.AreEqual(1, page.Questions.First().Options.Count(o => o.IsSelected == true));

        Assert.AreEqual("individual", page.Questions.First().Options.First(o => o.IsSelected == true).Value);
    }

    [TestMethod]
    public async Task OnTypeOfOrganisationPage_WhenTypeOfOrganisationIsNotSet_ThenNextPageIsStringEmpty()
    {
        _pageService.SetVisitedPages(new VisitedPages() { TypeOfOrganisation = TypeOfOrganisation.NotSet });

        Page page = await _pageService.GetPageAsync(PagePath.TypeOfOrganisation);

        Assert.IsTrue(string.IsNullOrEmpty(PageFinder.GetNextPath(page)));
    }

    [TestMethod]
    public async Task OnTypeOfOrganisationPage_WhenTypeOfOrganisationIsParentCompany_ThenNextPageIsAnnualTurnover()
    {
        _pageService.SetVisitedPages(new VisitedPages() { TypeOfOrganisation = TypeOfOrganisation.ParentCompany });

        Page page = await _pageService.GetPageAsync(PagePath.TypeOfOrganisation);

        Assert.AreEqual(PagePath.AnnualTurnover, PageFinder.GetNextPath(page));
    }

    [TestMethod]
    public async Task OnTypeOfOrganisationPage_WhenTypeOfOrganisationIsSubsidiary_ThenNextPageIsAnnualTurnover()
    {
        _pageService.SetVisitedPages(new VisitedPages() { TypeOfOrganisation = TypeOfOrganisation.Subsidiary });

        Page page = await _pageService.GetPageAsync(PagePath.TypeOfOrganisation);

        Assert.AreEqual(PagePath.AnnualTurnover, PageFinder.GetNextPath(page));
    }

    [TestMethod]
    public async Task OnTypeOfOrganisationPage_WhenTypeOfOrganisationIsIndividualCompany_ThenNextPageIsAnnualTurnover()
    {
        _pageService.SetVisitedPages(new VisitedPages() { TypeOfOrganisation = TypeOfOrganisation.IndividualCompany });

        Page page = await _pageService.GetPageAsync(PagePath.TypeOfOrganisation);

        Assert.AreEqual(PagePath.AnnualTurnover, PageFinder.GetNextPath(page));
    }

    [TestMethod]
    public async Task OnAnnualTurnoverPage_WhenAnnualTurnoverIsNotSet_ThenQuestionOptionsAreNotSelected()
    {
        _pageService.SetVisitedPages(new VisitedPages()
        {
            TypeOfOrganisation = TypeOfOrganisation.IndividualCompany,
            AnnualTurnover = AnnualTurnover.NotSet
        });

        Page page = await _pageService.GetPageAsync(PagePath.AnnualTurnover);

        Assert.IsNotNull(page);

        page.Questions.First().Options.Where(o => o.IsSelected is not null).Should().BeEmpty();
    }
}