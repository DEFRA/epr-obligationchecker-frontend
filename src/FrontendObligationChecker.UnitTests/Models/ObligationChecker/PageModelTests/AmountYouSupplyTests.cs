using FluentAssertions;

using FrontendObligationChecker.Models.ObligationChecker;
using FrontendObligationChecker.Services.PageService;
using FrontendObligationChecker.UnitTests.Helpers;

using AmountYouSupply = FrontendObligationChecker.ViewModels.AmountYouSupply;
using YesNo = FrontendObligationChecker.UnitTests.Helpers.YesNo;

namespace FrontendObligationChecker.UnitTests.Models.ObligationChecker.PageModelTests;

[TestClass]
public class AmountYouSupplyTests
{
    private PageService _pageService;

    [TestInitialize]
    public void TestInit()
    {
        _pageService = TestPageService.GetPageService();
    }

    [TestMethod]
    public async Task OnAmountYouSupplyPage_WhenOnPreviousPagesSellerAndSomeOtherActivityAndAlsoDrinksWereSelected_ThenPagePropertiesReflectTheirValues()
    {
        await _pageService.SetVisitedPages(new VisitedPages()
        {
            TypeOfOrganisation = TypeOfOrganisation.IndividualCompany,
            AnnualTurnover = AnnualTurnover.OverTwoMillion,
            OwnBrand = YesNo.Yes,
            UnbrandedPackaging = YesNo.No,
            ImportingProducts = YesNo.No,
            SellingEmptyPackaging = YesNo.No,
            HiringLoaning = YesNo.No,
            OnlineMarketplace = YesNo.No,
            SupplyingFilledPackaging = YesNo.Yes,
            PlaceDrinksOnMarket = YesNo.Yes,
            MaterialsForDrinksContainers = MaterialsForDrinksContainers.GlassBottles | MaterialsForDrinksContainers.AluminiumCans,
            ContainerVolume = YesNo.Yes
        });

        Page page = await _pageService.GetPageAsync(PagePath.AmountYouSupply);

        var amountYouHandle = new AmountYouSupply(page);

        Assert.IsTrue(amountYouHandle.HasSingleActivity);
        Assert.IsFalse(amountYouHandle.IsParentCompany);
        Assert.IsTrue(amountYouHandle.HasSingleUseDrinkContainers);
        amountYouHandle.NonSellerYesActivities.Should().NotBeEmpty();
    }

    [TestMethod]
    [DataRow(1, true)]
    [DataRow(2, false)]
    [DataRow(3, false)]
    public async Task NonSellerYesActivities_UseTextForAmountYouSupplyInsteadOfAlternateTitleWhereTheyExist(int selectedAsYes, bool matchingAltTitle)
    {
        await _pageService.SetVisitedPages(new VisitedPages()
        {
            TypeOfOrganisation = TypeOfOrganisation.IndividualCompany,
            AnnualTurnover = AnnualTurnover.OverTwoMillion,
            OwnBrand = selectedAsYes == 1 ? YesNo.Yes : YesNo.No,
            UnbrandedPackaging = YesNo.No,
            ImportingProducts = selectedAsYes == 2 ? YesNo.Yes : YesNo.No,
            SellingEmptyPackaging = selectedAsYes == 3 ? YesNo.Yes : YesNo.No,
        });

        Page page = await _pageService.GetPageAsync(PagePath.AmountYouSupply);

        var nonSellerYesPages = new[]
        {
            PagePath.OwnBrand,
            PagePath.ImportingProducts,
            PagePath.SupplyingEmptyPackaging,
        };
        var yesQuestion = page.SessionPages
            .Find(p => nonSellerYesPages.Contains(p.Path) && p.FirstQuestion.Answer == FrontendObligationChecker.Models.ObligationChecker.YesNo.Yes)
            .FirstQuestion;
        var expectedResourceString = matchingAltTitle ? yesQuestion.AlternateTitle : yesQuestion.AmountHandlePageText;

        var amountYouHandle = new AmountYouSupply(page);

        amountYouHandle.NonSellerYesActivities.Should().OnlyContain(s => s == expectedResourceString);
    }
}