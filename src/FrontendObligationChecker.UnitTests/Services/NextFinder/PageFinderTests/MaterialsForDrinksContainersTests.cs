using FrontendObligationChecker.Models.ObligationChecker;
using FrontendObligationChecker.Services.NextFinder;
using FrontendObligationChecker.UnitTests.Helpers;

using YesNo = FrontendObligationChecker.UnitTests.Helpers.YesNo;

namespace FrontendObligationChecker.UnitTests.Services.NextFinder.PageFinderTests;

[TestClass]
public class MaterialsForDrinksContainersTests
{
    private FrontendObligationChecker.Services.PageService.PageService _pageService;

    [TestInitialize]
    public void TestInit()
    {
        _pageService = TestPageService.GetPageService();
    }

    [TestMethod]
    public async Task OnMaterialsForDrinksContainersPage_WhenHasSomeMaterialsSelected_ThenNextPathIsContainerVolume()
    {
        await _pageService.SetVisitedPages(new VisitedPages()
        {
            TypeOfOrganisation = TypeOfOrganisation.IndividualCompany,
            AnnualTurnover = AnnualTurnover.OverTwoMillion,
            OwnBrand = YesNo.Yes,
            UnbrandedPackaging = YesNo.No,
            ImportingProducts = YesNo.No,
            SellingEmptyPackaging = YesNo.No,
            HiringLoaning = YesNo.Yes,
            OnlineMarketplace = YesNo.No,
            SupplyingFilledPackaging = YesNo.Yes,
            PlaceDrinksOnMarket = YesNo.Yes,
            MaterialsForDrinksContainers = MaterialsForDrinksContainers.AluminiumCans
        });

        Page page = await _pageService.GetPageAsync(PagePath.MaterialsForDrinksContainers);

        string nextPath = PageFinder.GetNextPath(page);

        Assert.AreEqual(PagePath.ContainerVolume, nextPath);
    }

    [TestMethod]
    public async Task OnMaterialsForDrinksContainersPage_WhenHasNoneOfMaterialsSelected_ThenNextPathIsWhatYouNeedToDo()
    {
        await _pageService.SetVisitedPages(new VisitedPages()
        {
            TypeOfOrganisation = TypeOfOrganisation.IndividualCompany,
            AnnualTurnover = AnnualTurnover.OverTwoMillion,
            OwnBrand = YesNo.Yes,
            UnbrandedPackaging = YesNo.No,
            ImportingProducts = YesNo.No,
            SellingEmptyPackaging = YesNo.No,
            HiringLoaning = YesNo.Yes,
            OnlineMarketplace = YesNo.No,
            SupplyingFilledPackaging = YesNo.Yes,
            PlaceDrinksOnMarket = YesNo.Yes,
            MaterialsForDrinksContainers = MaterialsForDrinksContainers.None
        });

        Page page = await _pageService.GetPageAsync(PagePath.MaterialsForDrinksContainers);

        page.FirstQuestion.Options.Where(o => o.Title == "SingleQuestion.MaterialsForDrinksContainers.None").FirstOrDefault().IsSelected = true;

        string nextPath = PageFinder.GetNextPath(page);

        Assert.AreEqual(PagePath.WhatYouNeedToDo, nextPath);
    }

    [TestMethod]
    public async Task OnMaterialsForDrinksContainersPage_WhenHasNoneOfMaterialsSelectedAndIsSellerOnly_ThenNextPathIsWhatYouNeedToDo()
    {
        await _pageService.SetVisitedPages(new VisitedPages()
        {
            TypeOfOrganisation = TypeOfOrganisation.IndividualCompany,
            AnnualTurnover = AnnualTurnover.OverTwoMillion,
            AmountYouSupply = AmountYouSupply.HandleUnder25Tonnes,
            OwnBrand = YesNo.Yes,
            UnbrandedPackaging = YesNo.No,
            ImportingProducts = YesNo.No,
            SellingEmptyPackaging = YesNo.No,
            HiringLoaning = YesNo.No,
            OnlineMarketplace = YesNo.No,
            SupplyingFilledPackaging = YesNo.Yes,
            PlaceDrinksOnMarket = YesNo.Yes,
            MaterialsForDrinksContainers = MaterialsForDrinksContainers.None
        });

        Page page = await _pageService.GetPageAsync(PagePath.MaterialsForDrinksContainers);

        page.FirstQuestion.Options.Where(o => o.Title == "SingleQuestion.MaterialsForDrinksContainers.None").FirstOrDefault().IsSelected = true;

        string nextPath = PageFinder.GetNextPath(page);

        Assert.AreEqual(PagePath.WhatYouNeedToDo, nextPath);
    }
}