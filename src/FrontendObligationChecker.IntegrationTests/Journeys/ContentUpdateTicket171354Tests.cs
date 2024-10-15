using FluentAssertions;

using FrontendObligationChecker.IntegrationTests.Extensions;
using FrontendObligationChecker.Models.ObligationChecker;
using FrontendObligationChecker.UnitTests.Helpers;

using YesNo = FrontendObligationChecker.UnitTests.Helpers.YesNo;

namespace FrontendObligationChecker.IntegrationTests.Journeys;

[TestClass]
public class ContentUpdateTicket171354Tests : TestBase
{
    // -- TODO -- Rafactor this.

    /*
    [TestMethod]
    public async Task OwnBrand_PageContents_AreUpdated()
    {
        await PostForm(PagePath.TypeOfOrganisation, new PageForm(TypeOfOrganisation.IndividualCompany).FormUrlEncodedContent);
        var page = await PostForm(PagePath.AnnualTurnover, new PageForm(AnnualTurnover.OverTwoMillion).FormUrlEncodedContent);
        var pageContent = await page.Content.ReadAsStringAsync();

        pageContent.Should().Contain("<title>Supplying goods to the UK market under your own brand</title>");
        pageContent.Should().Contain("You supply goods under your own brand even if you pay or license another company to do any of the following for you");
        pageContent.Should().Contain("import goods for you");
    }
    */

    // -- TODO -- Rafactor this.

    /*
    [TestMethod]
    public async Task UnbrandedPackaging_PageContents_AreUpdated()
    {
        await PostForm(PagePath.TypeOfOrganisation, new PageForm(TypeOfOrganisation.IndividualCompany).FormUrlEncodedContent);
        await PostForm(PagePath.AnnualTurnover, new PageForm(AnnualTurnover.OverTwoMillion).FormUrlEncodedContent);
        var page = await PostForm(PagePath.OwnBrand, new PageForm(QuestionKey.OwnBrand, YesNo.Yes).FormUrlEncodedContent);
        var pageContent = await page.Content.ReadAsStringAsync();

        pageContent.Should().Contain("<title>Placing goods into packaging that's unbranded when it's supplied - GOV.UK</title>".ApostropheToHex());
        pageContent.Should().Contain("Do you place goods into packaging that's unbranded when it's supplied?".ApostropheToHex());
    }
    */

    [TestMethod]
    public async Task ImportingProducts_PageContents_AreUpdated()
    {
        await PostForm(PagePath.TypeOfOrganisation, new PageForm(TypeOfOrganisation.IndividualCompany).FormUrlEncodedContent);
        await PostForm(PagePath.AnnualTurnover, new PageForm(AnnualTurnover.OverTwoMillion).FormUrlEncodedContent);
        await PostForm(PagePath.OwnBrand, new PageForm(QuestionKey.OwnBrand, YesNo.Yes).FormUrlEncodedContent);
        var page = await PostForm(PagePath.UnbrandedPackaging, new PageForm(QuestionKey.UnbrandedPackaging, YesNo.Yes).FormUrlEncodedContent);
        var pageContent = await page.Content.ReadAsStringAsync();

        pageContent.Should().Contain("products from outside the UK that are in packaging and go on to supply these products to the UK market");
        pageContent.Should().Contain("Find out what's classed as a 'large' organisation (opens in a new tab)");
    }

    [TestMethod]
    public async Task SellingEmptyPackaging_PageContents_AreUpdated()
    {
        await PostForm(PagePath.TypeOfOrganisation, new PageForm(TypeOfOrganisation.IndividualCompany).FormUrlEncodedContent);
        await PostForm(PagePath.AnnualTurnover, new PageForm(AnnualTurnover.OverTwoMillion).FormUrlEncodedContent);
        await PostForm(PagePath.OwnBrand, new PageForm(QuestionKey.OwnBrand, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.UnbrandedPackaging, new PageForm(QuestionKey.UnbrandedPackaging, YesNo.Yes).FormUrlEncodedContent);
        var page = await PostForm(PagePath.ImportingProducts, new PageForm(QuestionKey.ImportingProducts, YesNo.Yes).FormUrlEncodedContent);
        var pageContent = await page.Content.ReadAsStringAsync();

        pageContent.Should().Contain("<title>Supplying empty packaging - GOV.UK</title>");
        pageContent.Should().Contain("supply or import more than 50 tonnes of empty packaging or packaged goods");
    }

    [TestMethod]
    public async Task OnlineMarketplace_PageContents_AreUpdated()
    {
        await PostForm(PagePath.TypeOfOrganisation, new PageForm(TypeOfOrganisation.IndividualCompany).FormUrlEncodedContent);
        await PostForm(PagePath.AnnualTurnover, new PageForm(AnnualTurnover.OverTwoMillion).FormUrlEncodedContent);
        await PostForm(PagePath.OwnBrand, new PageForm(QuestionKey.OwnBrand, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.UnbrandedPackaging, new PageForm(QuestionKey.UnbrandedPackaging, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.ImportingProducts, new PageForm(QuestionKey.ImportingProducts, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.SupplyingEmptyPackaging, new PageForm(QuestionKey.SellingEmptyPackaging, YesNo.Yes).FormUrlEncodedContent);
        var page = await PostForm(PagePath.HiringLoaning, new PageForm(QuestionKey.HiringLoaning, YesNo.Yes).FormUrlEncodedContent);
        var pageContent = await page.Content.ReadAsStringAsync();

        pageContent.Should().Contain("Under EPR for packaging, this is when you operate a website or app that allows non-UK businesses to sell their goods into the UK.");
        pageContent.Should().Contain("If your organisation owns a website or app that sells goods from UK organisations only, this is not classed as owning an online marketplace.");
    }

    [TestMethod]
    public async Task FilledPackaging_PageContents_AreUpdated()
    {
        await PostForm(PagePath.TypeOfOrganisation, new PageForm(TypeOfOrganisation.IndividualCompany).FormUrlEncodedContent);
        await PostForm(PagePath.AnnualTurnover, new PageForm(AnnualTurnover.OverTwoMillion).FormUrlEncodedContent);
        await PostForm(PagePath.OwnBrand, new PageForm(QuestionKey.OwnBrand, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.UnbrandedPackaging, new PageForm(QuestionKey.UnbrandedPackaging, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.ImportingProducts, new PageForm(QuestionKey.ImportingProducts, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.SupplyingEmptyPackaging, new PageForm(QuestionKey.SellingEmptyPackaging, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.HiringLoaning, new PageForm(QuestionKey.HiringLoaning, YesNo.Yes).FormUrlEncodedContent);
        var page = await PostForm(PagePath.OnlineMarketplace, new PageForm(QuestionKey.OnlineMarketplace, YesNo.Yes).FormUrlEncodedContent);
        var pageContent = await page.Content.ReadAsStringAsync();

        pageContent.Should().Contain("<title>Supplying filled packaging to individual customers - GOV.UK</title>");
        pageContent.Should().Contain("This refers to any organisations that supply filled packaging directly to individual customers in the UK, where they are the end user of the packaging.");
        pageContent.Should().Contain("It does not refer to filled packaging supplied to businesses.");
        pageContent.Should().Contain("Do you supply filled packaging to individual customers?");
        pageContent.Should().NotContain(" consumers");
    }

    [TestMethod]
    public async Task MaterialsForDrinksContainers_PageContents_AreUpdated()
    {
        await PostForm(PagePath.TypeOfOrganisation, new PageForm(TypeOfOrganisation.IndividualCompany).FormUrlEncodedContent);
        await PostForm(PagePath.AnnualTurnover, new PageForm(AnnualTurnover.OverTwoMillion).FormUrlEncodedContent);
        await PostForm(PagePath.OwnBrand, new PageForm(QuestionKey.OwnBrand, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.UnbrandedPackaging, new PageForm(QuestionKey.UnbrandedPackaging, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.ImportingProducts, new PageForm(QuestionKey.ImportingProducts, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.SupplyingEmptyPackaging, new PageForm(QuestionKey.SellingEmptyPackaging, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.HiringLoaning, new PageForm(QuestionKey.HiringLoaning, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.OnlineMarketplace, new PageForm(QuestionKey.OnlineMarketplace, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.SupplyingFilledPackaging, new PageForm(QuestionKey.SellingEmptyPackaging, YesNo.Yes).FormUrlEncodedContent);
        var page = await PostForm(PagePath.PlaceDrinksOnMarket, new PageForm(QuestionKey.SingleUseContainersOnMarket, YesNo.Yes).FormUrlEncodedContent);
        var pageContent = await page.Content.ReadAsStringAsync();

        pageContent.Should().Contain("Polyethylene terephthalate (PET) plastic");
        pageContent.Should().NotContain("Polyethylene terephthalate (PET) plastic bottles");
    }

    [TestMethod]
    public async Task AmountYouSupply_PageContents_AreUpdated()
    {
        await PostForm(PagePath.TypeOfOrganisation, new PageForm(TypeOfOrganisation.IndividualCompany).FormUrlEncodedContent);
        await PostForm(PagePath.AnnualTurnover, new PageForm(AnnualTurnover.OverTwoMillion).FormUrlEncodedContent);
        await PostForm(PagePath.OwnBrand, new PageForm(QuestionKey.OwnBrand, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.UnbrandedPackaging, new PageForm(QuestionKey.UnbrandedPackaging, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.ImportingProducts, new PageForm(QuestionKey.ImportingProducts, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.SupplyingEmptyPackaging, new PageForm(QuestionKey.SellingEmptyPackaging, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.HiringLoaning, new PageForm(QuestionKey.HiringLoaning, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.OnlineMarketplace, new PageForm(QuestionKey.OnlineMarketplace, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.SupplyingFilledPackaging, new PageForm(QuestionKey.SellingEmptyPackaging, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.PlaceDrinksOnMarket, new PageForm(QuestionKey.SingleUseContainersOnMarket, YesNo.Yes).FormUrlEncodedContent);
        const MaterialsForDrinksContainers allMaterialsForDrinksContainersSelected = MaterialsForDrinksContainers.AluminiumCans |
                                                                                     MaterialsForDrinksContainers.GlassBottles |
                                                                                     MaterialsForDrinksContainers.PlasticBottles |
                                                                                     MaterialsForDrinksContainers.SteelCans;
        await PostForm(PagePath.MaterialsForDrinksContainers, new PageForm(allMaterialsForDrinksContainersSelected).FormUrlEncodedContent);
        var page = await PostForm(PagePath.ContainerVolume, new PageForm(QuestionKey.ContainerVolume, YesNo.Yes).FormUrlEncodedContent);
        var pageContent = await page.Content.ReadAsStringAsync();

        pageContent.Should().Contain("importing products in packaging which are supplied or discarded in the UK");
        pageContent.Should().Contain("supplying empty packaging to businesses that aren't classed as a large organisation".ApostropheToHex());
    }

    [TestMethod]
    public async Task NoActionsNeeded_AmountYouSupply_PageContents_Test()
    {
        await PostForm(PagePath.TypeOfOrganisation, new PageForm(TypeOfOrganisation.IndividualCompany).FormUrlEncodedContent);
        await PostForm(PagePath.AnnualTurnover, new PageForm(AnnualTurnover.OverTwoMillion).FormUrlEncodedContent);
        await PostForm(PagePath.OwnBrand, new PageForm(QuestionKey.OwnBrand, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.UnbrandedPackaging, new PageForm(QuestionKey.UnbrandedPackaging, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.ImportingProducts, new PageForm(QuestionKey.ImportingProducts, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.SupplyingEmptyPackaging, new PageForm(QuestionKey.SellingEmptyPackaging, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.HiringLoaning, new PageForm(QuestionKey.HiringLoaning, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.OnlineMarketplace, new PageForm(QuestionKey.OnlineMarketplace, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.SupplyingFilledPackaging, new PageForm(QuestionKey.SellingEmptyPackaging, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.PlaceDrinksOnMarket, new PageForm(QuestionKey.SingleUseContainersOnMarket, YesNo.Yes).FormUrlEncodedContent);
        const MaterialsForDrinksContainers allMaterialsForDrinksContainersSelected = MaterialsForDrinksContainers.AluminiumCans |
                                                                                     MaterialsForDrinksContainers.GlassBottles |
                                                                                     MaterialsForDrinksContainers.PlasticBottles |
                                                                                     MaterialsForDrinksContainers.SteelCans;
        await PostForm(PagePath.MaterialsForDrinksContainers, new PageForm(allMaterialsForDrinksContainersSelected).FormUrlEncodedContent);
        await PostForm(PagePath.ContainerVolume, new PageForm(QuestionKey.ContainerVolume, YesNo.Yes).FormUrlEncodedContent);
        var page = await PostForm(PagePath.AmountYouSupply, new PageForm(AmountYouSupply.HandleUnder25Tonnes).FormUrlEncodedContent);
        var pageContent = await page.Content.ReadAsStringAsync();

        pageContent.Should().Contain("This is because your organisation will handle or supply less than 25 tonnes of packaging in 2022.");
    }

    [TestMethod]
    public async Task WhatYouNeedToDo_PageContents_AreUpdated()
    {
        await PostForm(PagePath.TypeOfOrganisation, new PageForm(TypeOfOrganisation.IndividualCompany).FormUrlEncodedContent);
        await PostForm(PagePath.AnnualTurnover, new PageForm(AnnualTurnover.OverTwoMillion).FormUrlEncodedContent);
        await PostForm(PagePath.OwnBrand, new PageForm(QuestionKey.OwnBrand, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.UnbrandedPackaging, new PageForm(QuestionKey.UnbrandedPackaging, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.ImportingProducts, new PageForm(QuestionKey.ImportingProducts, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.SupplyingEmptyPackaging, new PageForm(QuestionKey.SellingEmptyPackaging, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.HiringLoaning, new PageForm(QuestionKey.HiringLoaning, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.OnlineMarketplace, new PageForm(QuestionKey.OnlineMarketplace, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.SupplyingFilledPackaging, new PageForm(QuestionKey.SellingEmptyPackaging, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.PlaceDrinksOnMarket, new PageForm(QuestionKey.SingleUseContainersOnMarket, YesNo.Yes).FormUrlEncodedContent);
        const MaterialsForDrinksContainers allMaterialsForDrinksContainersSelected = MaterialsForDrinksContainers.AluminiumCans |
                                                                                     MaterialsForDrinksContainers.GlassBottles |
                                                                                     MaterialsForDrinksContainers.PlasticBottles |
                                                                                     MaterialsForDrinksContainers.SteelCans;
        await PostForm(PagePath.MaterialsForDrinksContainers, new PageForm(allMaterialsForDrinksContainersSelected).FormUrlEncodedContent);
        await PostForm(PagePath.ContainerVolume, new PageForm(QuestionKey.ContainerVolume, YesNo.Yes).FormUrlEncodedContent);
        var page = await PostForm(PagePath.AmountYouSupply, new PageForm(AmountYouSupply.Handle50TonnesOrMore).FormUrlEncodedContent);
        var pageContent = await page.Content.ReadAsStringAsync();

        pageContent.Should().Contain("When to collect and report");
        pageContent.Should().NotContain("a charge to the environmental regulator");
        pageContent.Should().Contain("A PRN or PERN is evidence that packaging waste has been recycled");
        pageContent.Should().Contain("Nation data is information about which nation in the UK packaging is supplied in and which nation in the UK packaging is discarded in.");

        pageContent.Should().NotContain("importing products in packaging which are supplied or discarded in the UK");
        pageContent.Should().NotContain("supplying empty packaging to businesses that aren't classed as a large organisation".ApostropheToHex());
    }
}