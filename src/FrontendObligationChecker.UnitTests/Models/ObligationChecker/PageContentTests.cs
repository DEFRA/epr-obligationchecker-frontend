using FluentAssertions;
using FrontendObligationChecker.Models.ObligationChecker;
using FrontendObligationChecker.Services.PageService;
using FrontendObligationChecker.UnitTests.Helpers;

namespace FrontendObligationChecker.UnitTests.Models.ObligationChecker;

[TestClass]
public class PageContentTests
{
    private PageService _pageService;

    [TestInitialize]
    public void TestInit()
    {
        _pageService = TestPageService.GetPageService();
    }

    [TestMethod]
    public async Task PageService_WhenFirstPageIsLoadedWithoutAnySetup_ThenItsContentIsEmpty()
    {
        Page page = await _pageService.GetPageAsync(PagePath.TypeOfOrganisation);

        Assert.AreEqual(PagePath.TypeOfOrganisation, page.Path);
        Assert.IsNull(page.BackLinkPath);
        Assert.IsFalse(page.HasBackLink);
        Assert.IsNull(page.GetAlternateTitleFirstParagraph());
        Assert.IsNull(page.AdditionalDescription);
        Assert.IsNull(page.NextValue());
        page.GetAlternateTitlesSubContent().Should().BeEmpty();
        page.GetAlternateTitles().Should().BeEmpty();
        page.Errors.Should().BeEmpty();
    }
}