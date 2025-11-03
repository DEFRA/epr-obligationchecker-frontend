using FrontendObligationChecker.Models.ObligationChecker;
using FrontendObligationChecker.Services.PageService;
using FrontendObligationChecker.UnitTests.Helpers;

using YesNo = FrontendObligationChecker.UnitTests.Helpers.YesNo;

namespace FrontendObligationChecker.UnitTests.Models.ObligationChecker.PageModelTests;

[TestClass]
public class WhatYouNeedToDoTests
{
    private PageService _pageService;

    [TestInitialize]
    public void TestInit()
    {
        _pageService = TestPageService.GetPageService();
    }

    [TestMethod]
    public async Task OnWhatYouNeedToDoPage_WhenTurnoverWasOverTwoMillionAndHandle50TonnesOrMore_ThenCompanySizeIsLarge()
    {
        await _pageService.SetVisitedPages(new VisitedPages()
        {
            TypeOfOrganisation = TypeOfOrganisation.IndividualCompany,
            AnnualTurnover = AnnualTurnover.OverTwoMillion,
            OwnBrand = YesNo.Yes,
            UnbrandedPackaging = YesNo.Yes,
            ImportingProducts = YesNo.No,
            SellingEmptyPackaging = YesNo.No,
            HiringLoaning = YesNo.Yes,
            OnlineMarketplace = YesNo.Yes,
            SupplyingFilledPackaging = YesNo.No,
            PlaceDrinksOnMarket = YesNo.No,
            AmountYouSupply = AmountYouSupply.Handle50TonnesOrMore
        });

        Page page = await _pageService.GetPageAsync(PagePath.WhatYouNeedToDo);

        Assert.AreEqual(CompanySize.Large, page.CompanyModel.CompanySize);
    }

    [TestMethod]
    public async Task OnWhatYouNeedToDoPage_WhenTurnoverWasLessThanTwoMillion_ThenCompanySizeIsSmall()
    {
        await _pageService.SetVisitedPages(new VisitedPages()
        {
            TypeOfOrganisation = TypeOfOrganisation.IndividualCompany,
            AnnualTurnover = AnnualTurnover.OneMillionToTwoMillion,
            OwnBrand = YesNo.No,
            UnbrandedPackaging = YesNo.Yes,
            ImportingProducts = YesNo.No,
            SellingEmptyPackaging = YesNo.No,
            HiringLoaning = YesNo.Yes,
            OnlineMarketplace = YesNo.No,
            SupplyingFilledPackaging = YesNo.No,
            PlaceDrinksOnMarket = YesNo.No,
            AmountYouSupply = AmountYouSupply.Handle50TonnesOrMore
        });

        Page page = await _pageService.GetPageAsync(PagePath.WhatYouNeedToDo);

        Assert.AreEqual(CompanySize.Small, page.CompanyModel.CompanySize);
    }

    // AC2-A
    [TestMethod]
    [DataRow(YesNo.Yes, YesNo.Yes, AnnualTurnover.OverTwoMillion)]
    [DataRow(YesNo.Yes, YesNo.Yes, AnnualTurnover.OneMillionToTwoMillion)]
    [DataRow(YesNo.No, YesNo.Yes, AnnualTurnover.OverTwoMillion)]
    [DataRow(YesNo.No, YesNo.Yes, AnnualTurnover.OneMillionToTwoMillion)]
    [DataRow(YesNo.Yes, YesNo.No, AnnualTurnover.OverTwoMillion)]
    [DataRow(YesNo.Yes, YesNo.No, AnnualTurnover.OneMillionToTwoMillion)]
    public async Task OnWhatYouNeedToDoPage_WhenAmountYouHandleWasLessThan50Tonnes_ThenCompanySizeIsSmall(
        YesNo brandOwner, YesNo packerFiller, AnnualTurnover annualTurnover)
    {
        await _pageService.SetVisitedPages(new VisitedPages()
        {
            TypeOfOrganisation = TypeOfOrganisation.IndividualCompany,
            AnnualTurnover = annualTurnover,
            OwnBrand = brandOwner,
            UnbrandedPackaging = packerFiller,
            ImportingProducts = YesNo.No,
            SellingEmptyPackaging = YesNo.No,
            HiringLoaning = YesNo.No,
            OnlineMarketplace = YesNo.No,
            SupplyingFilledPackaging = YesNo.No,
            PlaceDrinksOnMarket = YesNo.No,
            AmountYouSupply = AmountYouSupply.Handle25To50Tonnes
        });

        Page page = await _pageService.GetPageAsync(PagePath.WhatYouNeedToDo);

        Assert.AreEqual(CompanySize.Small, page.CompanyModel.CompanySize);
        Assert.IsFalse(page.CompanyModel.RequiresNationData);
    }

    // AC2-B
    [TestMethod]
    [DataRow(AnnualTurnover.OverTwoMillion)]
    [DataRow(AnnualTurnover.OneMillionToTwoMillion)]
    public async Task OnWhatYouNeedToDoPage_WhenSellerOnly_AmountYouHandleWasLessThan50Tonnes_ThenCompanySizeIsSmall(AnnualTurnover annualTurnover)
    {
        await _pageService.SetVisitedPages(new VisitedPages()
        {
            TypeOfOrganisation = TypeOfOrganisation.IndividualCompany,
            AnnualTurnover = annualTurnover,
            OwnBrand = YesNo.Yes,
            UnbrandedPackaging = YesNo.No,
            ImportingProducts = YesNo.No,
            SellingEmptyPackaging = YesNo.Yes,
            HiringLoaning = YesNo.No,
            OnlineMarketplace = YesNo.No,
            SupplyingFilledPackaging = YesNo.No,
            PlaceDrinksOnMarket = YesNo.No,
            AmountYouSupply = AmountYouSupply.Handle25To50Tonnes
        });

        Page page = await _pageService.GetPageAsync(PagePath.WhatYouNeedToDo);

        Assert.AreEqual(CompanySize.Small, page.CompanyModel.CompanySize);
    }

    // AC2-C
    [TestMethod]
    [DataRow(YesNo.Yes, YesNo.Yes, YesNo.Yes, YesNo.Yes, YesNo.Yes, YesNo.Yes, YesNo.Yes, YesNo.Yes, AnnualTurnover.OneMillionToTwoMillion)]
    [DataRow(YesNo.Yes, YesNo.Yes, YesNo.Yes, YesNo.Yes, YesNo.Yes, YesNo.Yes, YesNo.Yes, YesNo.Yes, AnnualTurnover.OverTwoMillion)]
    public async Task OnWhatYouNeedToDoPage_WhenSellerOnly_AmountYouHandleWasLessThan50Tonnes_ThenCompanySizeIsSmall(
        YesNo ownBrand, YesNo unbranded, YesNo importing, YesNo seller, YesNo hiring, YesNo online,
        YesNo filledPackaging, YesNo drinksOnMarket, AnnualTurnover annualTurnover)
    {
        await _pageService.SetVisitedPages(new VisitedPages()
        {
            TypeOfOrganisation = TypeOfOrganisation.IndividualCompany,
            AnnualTurnover = annualTurnover,
            OwnBrand = ownBrand,
            UnbrandedPackaging = unbranded,
            ImportingProducts = importing,
            SellingEmptyPackaging = seller,
            HiringLoaning = hiring,
            OnlineMarketplace = online,
            SupplyingFilledPackaging = filledPackaging,
            PlaceDrinksOnMarket = drinksOnMarket,
            AmountYouSupply = AmountYouSupply.Handle25To50Tonnes
        });

        Page page = await _pageService.GetPageAsync(PagePath.WhatYouNeedToDo);

        Assert.AreEqual(CompanySize.Small, page.CompanyModel.CompanySize);
        Assert.IsTrue(page.CompanyModel.RequiresNationData);
        Assert.AreEqual(SellerType.NotSellerOnly, page.CompanyModel.SellerType);
    }

    // AC3-A
    [TestMethod]
    [DataRow(YesNo.Yes, YesNo.Yes)]
    [DataRow(YesNo.No, YesNo.Yes)]
    [DataRow(YesNo.Yes, YesNo.No)]
    public async Task OnWhatYouNeedToDoPage_WhenAmountYouHandleWasMoreThan50Tonnes_ThenCompanySizeIsSmall(
        YesNo brandOwner, YesNo packerFiller)
    {
        await _pageService.SetVisitedPages(new VisitedPages()
        {
            TypeOfOrganisation = TypeOfOrganisation.IndividualCompany,
            AnnualTurnover = AnnualTurnover.OneMillionToTwoMillion,
            OwnBrand = brandOwner,
            UnbrandedPackaging = packerFiller,
            ImportingProducts = YesNo.No,
            SellingEmptyPackaging = YesNo.No,
            HiringLoaning = YesNo.No,
            OnlineMarketplace = YesNo.No,
            SupplyingFilledPackaging = YesNo.No,
            PlaceDrinksOnMarket = YesNo.No,
            AmountYouSupply = AmountYouSupply.Handle50TonnesOrMore
        });

        Page page = await _pageService.GetPageAsync(PagePath.WhatYouNeedToDo);

        Assert.AreEqual(CompanySize.Small, page.CompanyModel.CompanySize);
        Assert.IsFalse(page.CompanyModel.RequiresNationData);
    }

    // AC3-B
    [TestMethod]
    public async Task OnWhatYouNeedToDoPage_WhenSellerOnly_AmountYouHandleWasMoreThan50Tonnes_ThenCompanySizeIsSmall()
    {
        await _pageService.SetVisitedPages(new VisitedPages()
        {
            TypeOfOrganisation = TypeOfOrganisation.IndividualCompany,
            AnnualTurnover = AnnualTurnover.OneMillionToTwoMillion,
            OwnBrand = YesNo.Yes,
            UnbrandedPackaging = YesNo.No,
            ImportingProducts = YesNo.No,
            SellingEmptyPackaging = YesNo.Yes,
            HiringLoaning = YesNo.No,
            OnlineMarketplace = YesNo.No,
            SupplyingFilledPackaging = YesNo.No,
            PlaceDrinksOnMarket = YesNo.No,
            AmountYouSupply = AmountYouSupply.Handle50TonnesOrMore
        });

        Page page = await _pageService.GetPageAsync(PagePath.WhatYouNeedToDo);

        Assert.AreEqual(CompanySize.Small, page.CompanyModel.CompanySize);
        Assert.IsTrue(page.CompanyModel.RequiresNationData);
    }

    // AC3-C
    [TestMethod]
    [DataRow(YesNo.Yes, YesNo.Yes, YesNo.Yes, YesNo.Yes, YesNo.Yes, YesNo.Yes, YesNo.Yes, YesNo.Yes)]
    [DataRow(YesNo.Yes, YesNo.Yes, YesNo.Yes, YesNo.No, YesNo.No, YesNo.No, YesNo.Yes, YesNo.Yes)]
    public async Task OnWhatYouNeedToDoPage_WhenAnyCombination_AmountYouHandleWasAtLeast50Tonnes_ThenCompanySizeIsSmall(
        YesNo ownBrand, YesNo unbranded, YesNo importing, YesNo seller, YesNo hiring, YesNo online, YesNo filledPackaging, YesNo drinksOnMarket)
    {
        await _pageService.SetVisitedPages(new VisitedPages()
        {
            TypeOfOrganisation = TypeOfOrganisation.IndividualCompany,
            AnnualTurnover = AnnualTurnover.OneMillionToTwoMillion,
            OwnBrand = ownBrand,
            UnbrandedPackaging = unbranded,
            ImportingProducts = importing,
            SellingEmptyPackaging = seller,
            HiringLoaning = hiring,
            OnlineMarketplace = online,
            SupplyingFilledPackaging = filledPackaging,
            PlaceDrinksOnMarket = drinksOnMarket,
            AmountYouSupply = AmountYouSupply.Handle25To50Tonnes
        });

        Page page = await _pageService.GetPageAsync(PagePath.WhatYouNeedToDo);

        Assert.AreEqual(CompanySize.Small, page.CompanyModel.CompanySize);
        Assert.IsTrue(page.CompanyModel.RequiresNationData);
        Assert.AreEqual(SellerType.NotSellerOnly, page.CompanyModel.SellerType);
    }

    // AC4-A
    [TestMethod]
    [DataRow(YesNo.Yes, YesNo.Yes)]
    [DataRow(YesNo.Yes, YesNo.No)]
    public async Task OnWhatYouNeedToDoPage_BrandOwner_AndOr_PackerFiller_Only_OverTwoMillion_AtLeast50Tonnes_ThenCompanySizeIsLarge(
        YesNo brandOwner, YesNo packerFiller)
    {
        await _pageService.SetVisitedPages(new VisitedPages()
        {
            TypeOfOrganisation = TypeOfOrganisation.IndividualCompany,
            AnnualTurnover = AnnualTurnover.OverTwoMillion,
            OwnBrand = brandOwner,
            UnbrandedPackaging = packerFiller,
            ImportingProducts = YesNo.No,
            SellingEmptyPackaging = YesNo.No,
            HiringLoaning = YesNo.No,
            OnlineMarketplace = YesNo.No,
            SupplyingFilledPackaging = YesNo.No,
            PlaceDrinksOnMarket = YesNo.No,
            AmountYouSupply = AmountYouSupply.Handle50TonnesOrMore
        });

        Page page = await _pageService.GetPageAsync(PagePath.WhatYouNeedToDo);

        Assert.AreEqual(CompanySize.Large, page.CompanyModel.CompanySize);
        Assert.IsFalse(page.CompanyModel.RequiresNationData);
        Assert.AreEqual(SellerType.NotSellerOnly, page.CompanyModel.SellerType);
    }

    // AC4-B
    [TestMethod]
    public async Task OnWhatYouNeedToDoPage_SellerOnly_OverTwoMillion_AtLeast50Tonnes_ThenCompanySizeIsLarge()
    {
        await _pageService.SetVisitedPages(new VisitedPages()
        {
            TypeOfOrganisation = TypeOfOrganisation.IndividualCompany,
            AnnualTurnover = AnnualTurnover.OverTwoMillion,
            OwnBrand = YesNo.Yes,
            UnbrandedPackaging = YesNo.No,
            ImportingProducts = YesNo.No,
            SellingEmptyPackaging = YesNo.Yes,
            HiringLoaning = YesNo.No,
            OnlineMarketplace = YesNo.No,
            SupplyingFilledPackaging = YesNo.No,
            PlaceDrinksOnMarket = YesNo.No,
            AmountYouSupply = AmountYouSupply.Handle50TonnesOrMore
        });

        Page page = await _pageService.GetPageAsync(PagePath.WhatYouNeedToDo);

        Assert.AreEqual(CompanySize.Large, page.CompanyModel.CompanySize);
        Assert.IsTrue(page.CompanyModel.RequiresNationData);
    }

    // AC4-C
    [TestMethod]
    [DataRow(YesNo.Yes, YesNo.Yes, YesNo.Yes, YesNo.Yes, YesNo.Yes, YesNo.Yes, YesNo.Yes, YesNo.Yes)]
    [DataRow(YesNo.Yes, YesNo.Yes, YesNo.Yes, YesNo.No,  YesNo.No,  YesNo.No,  YesNo.No,  YesNo.No)]
    [DataRow(YesNo.Yes, YesNo.Yes, YesNo.Yes, YesNo.No,  YesNo.No,  YesNo.No,  YesNo.Yes, YesNo.Yes)]
    public async Task OnWhatYouNeedToDoPage_OtherCombinations_Over2Million_Over50Tonnes_ThenCompanySizeIsLarge(
        YesNo ownBrand, YesNo unbranded, YesNo seller, YesNo importing, YesNo hiring, YesNo online, YesNo filledPackaging, YesNo drinksOnMarket)
    {
        await _pageService.SetVisitedPages(new VisitedPages()
        {
            TypeOfOrganisation = TypeOfOrganisation.IndividualCompany,
            AnnualTurnover = AnnualTurnover.OverTwoMillion,
            OwnBrand = ownBrand,
            UnbrandedPackaging = unbranded,
            ImportingProducts = importing,
            SellingEmptyPackaging = seller,
            HiringLoaning = hiring,
            OnlineMarketplace = online,
            SupplyingFilledPackaging = filledPackaging,
            PlaceDrinksOnMarket = drinksOnMarket,
            MaterialsForDrinksContainers = MaterialsForDrinksContainers.None,
            AmountYouSupply = AmountYouSupply.Handle50TonnesOrMore
        });

        Page page = await _pageService.GetPageAsync(PagePath.WhatYouNeedToDo);

        Assert.AreEqual(CompanySize.Large, page.CompanyModel.CompanySize);
        Assert.IsTrue(page.CompanyModel.RequiresNationData);
    }

    [TestMethod]
    public async Task OnWhatYouNeedToDoPage_WhenMultipleHandleSupplyPackagingOptionsWereYes_ThenSellerTypeIsNotSellerOnly()
    {
        await _pageService.SetVisitedPages(new VisitedPages()
        {
            TypeOfOrganisation = TypeOfOrganisation.IndividualCompany,
            AnnualTurnover = AnnualTurnover.OverTwoMillion,
            OwnBrand = YesNo.No,
            UnbrandedPackaging = YesNo.No,
            ImportingProducts = YesNo.No,
            SellingEmptyPackaging = YesNo.No,
            HiringLoaning = YesNo.Yes,
            OnlineMarketplace = YesNo.Yes,
            SupplyingFilledPackaging = YesNo.No,
            PlaceDrinksOnMarket = YesNo.No,
            AmountYouSupply = AmountYouSupply.Handle50TonnesOrMore
        });

        Page page = await _pageService.GetPageAsync(PagePath.WhatYouNeedToDo);

        Assert.AreEqual(SellerType.NotSellerOnly, page.CompanyModel.SellerType);
    }

    [TestMethod]
    public async Task OnWhatYouNeedToDoPage_WhenOnlyHandleSupplyPackaging_EndConsumerPackagingSellerWasYes_ThenSellerTypeIsSellerOnly()
    {
        await _pageService.SetVisitedPages(new VisitedPages()
        {
            TypeOfOrganisation = TypeOfOrganisation.IndividualCompany,
            AnnualTurnover = AnnualTurnover.OverTwoMillion,
            OwnBrand = YesNo.Yes,
            UnbrandedPackaging = YesNo.No,
            ImportingProducts = YesNo.No,
            SellingEmptyPackaging = YesNo.Yes,
            HiringLoaning = YesNo.No,
            OnlineMarketplace = YesNo.No,
            SupplyingFilledPackaging = YesNo.No,
            PlaceDrinksOnMarket = YesNo.No,
            AmountYouSupply = AmountYouSupply.Handle50TonnesOrMore
        });

        Page page = await _pageService.GetPageAsync(PagePath.WhatYouNeedToDo);

        Assert.AreEqual(SellerType.NotSellerOnly, page.CompanyModel.SellerType);
    }
}