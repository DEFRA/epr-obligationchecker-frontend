using System.Web;
using FluentAssertions;

using FrontendObligationChecker.IntegrationTests.Extensions;
using FrontendObligationChecker.Models.ObligationChecker;
using FrontendObligationChecker.UnitTests.Helpers;

using YesNo = FrontendObligationChecker.UnitTests.Helpers.YesNo;

namespace FrontendObligationChecker.IntegrationTests.Journeys;

[TestClass]
public class ContentUpdateTicket171354Tests : TestBase
{
    [TestMethod]
    public async Task AnnualTurnover_PageContents_AreUpdated()
    {
        var page = await PostForm(PagePath.TypeOfOrganisation, new PageForm(TypeOfOrganisation.IndividualCompany).FormUrlEncodedContent);

        var pageContent = await page.Content.ReadAsStringAsync();

        pageContent.Should().Contain("What was your group’s last reported turnover?".CurvedApostropheToHex());
    }

    [TestMethod]
    public async Task OwnBrand_PageContents_AreUpdated()
    {
        await PostForm(PagePath.TypeOfOrganisation, new PageForm(TypeOfOrganisation.IndividualCompany).FormUrlEncodedContent);
        await PostForm(PagePath.AnnualTurnover, new PageForm(AnnualTurnover.OverTwoMillion).FormUrlEncodedContent);
        var page = await PostForm(PagePath.AmountYouSupply, new PageForm(AmountYouSupply.Handle50TonnesOrMore).FormUrlEncodedContent);

        var pageContent = await page.Content.ReadAsStringAsync();

        pageContent.Should().Contain("<title>Do you supply goods to the UK market under your own brand? - GOV.UK</title>");
        pageContent.Should().Contain("A brand is something your business could be visibly identified by, including:");
        pageContent.Should().Contain("You supply goods under your own brand even if you pay or license another company to:");

        pageContent.Should().NotContain("You supply goods under your own brand even if you pay or license another company to do any of the following for you");
        pageContent.Should().NotContain("import goods for you");
    }

    [TestMethod]
    public async Task UnbrandedPackaging_PageContents_AreUpdated()
    {
        await PostForm(PagePath.TypeOfOrganisation, new PageForm(TypeOfOrganisation.IndividualCompany).FormUrlEncodedContent);
        await PostForm(PagePath.AnnualTurnover, new PageForm(AnnualTurnover.OverTwoMillion).FormUrlEncodedContent);
        await PostForm(PagePath.AmountYouSupply, new PageForm(AmountYouSupply.Handle50TonnesOrMore).FormUrlEncodedContent);
        var page = await PostForm(PagePath.OwnBrand, new PageForm(QuestionKey.OwnBrand, YesNo.Yes).FormUrlEncodedContent);
        var pageContent = await page.Content.ReadAsStringAsync();

        pageContent.Should().Contain("<title>Do you place goods for sale into packaging? - GOV.UK</title>".ApostropheToHex());
        pageContent.Should().Contain("Do you place goods for sale into packaging?");
        pageContent.Should().Contain("This could be goods you packaged for your own organisation or for another organisation.");

        pageContent.Should().NotContain("Do you place goods into packaging that's unbranded when it's supplied?".ApostropheToHex());
        pageContent.Should().NotContain("This is when you place goods into packaging and that packaging is unbranded when it's supplied.".ApostropheToHex());
    }

    [TestMethod]
    public async Task ImportingProducts_PageContents_AreUpdated()
    {
        await PostForm(PagePath.TypeOfOrganisation, new PageForm(TypeOfOrganisation.IndividualCompany).FormUrlEncodedContent);
        await PostForm(PagePath.AnnualTurnover, new PageForm(AnnualTurnover.OverTwoMillion).FormUrlEncodedContent);
        await PostForm(PagePath.AmountYouSupply, new PageForm(AmountYouSupply.Handle50TonnesOrMore).FormUrlEncodedContent);
        await PostForm(PagePath.OwnBrand, new PageForm(QuestionKey.OwnBrand, YesNo.Yes).FormUrlEncodedContent);
        var page = await PostForm(PagePath.UnbrandedPackaging, new PageForm(QuestionKey.UnbrandedPackaging, YesNo.Yes).FormUrlEncodedContent);
        var pageContent = await page.Content.ReadAsStringAsync();

        pageContent.Should().Contain("This also applies to you if you discard the packaging before selling the goods");
        pageContent.Should().Contain("You are not classed as an organisation that imports products in packaging if you import filled packaging that is:");

        pageContent.Should().NotContain("products from outside the UK that are in packaging and go on to supply these products to the UK market");
        pageContent.Should().NotContain("Find out what's classed as a 'large' organisation (opens in a new tab)".ApostropheToHex());
    }

    [TestMethod]
    public async Task SellingEmptyPackaging_PageContents_AreUpdated()
    {
        await PostForm(PagePath.TypeOfOrganisation, new PageForm(TypeOfOrganisation.IndividualCompany).FormUrlEncodedContent);
        await PostForm(PagePath.AnnualTurnover, new PageForm(AnnualTurnover.OverTwoMillion).FormUrlEncodedContent);
        await PostForm(PagePath.AmountYouSupply, new PageForm(AmountYouSupply.Handle50TonnesOrMore).FormUrlEncodedContent);
        await PostForm(PagePath.OwnBrand, new PageForm(QuestionKey.OwnBrand, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.UnbrandedPackaging, new PageForm(QuestionKey.UnbrandedPackaging, YesNo.Yes).FormUrlEncodedContent);
        var page = await PostForm(PagePath.ImportingProducts, new PageForm(QuestionKey.ImportingProducts, YesNo.Yes).FormUrlEncodedContent);
        var pageContent = await page.Content.ReadAsStringAsync();

        pageContent.Should().Contain("<title>Do you supply empty packaging to organisations which are not classed as 'large'? - GOV.UK</title>".ApostropheToHex());
        pageContent.Should().Contain("Under EPR, large organisations:".ApostropheToHex());

        pageContent.Should().NotContain("supply or import more than 50 tonnes of empty packaging or packaged goods");
    }

    [TestMethod]
    public async Task OnlineMarketplace_PageContents_AreUpdated()
    {
        await PostForm(PagePath.TypeOfOrganisation, new PageForm(TypeOfOrganisation.IndividualCompany).FormUrlEncodedContent);
        await PostForm(PagePath.AnnualTurnover, new PageForm(AnnualTurnover.OverTwoMillion).FormUrlEncodedContent);
        await PostForm(PagePath.AmountYouSupply, new PageForm(AmountYouSupply.Handle50TonnesOrMore).FormUrlEncodedContent);
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
        await PostForm(PagePath.AmountYouSupply, new PageForm(AmountYouSupply.Handle50TonnesOrMore).FormUrlEncodedContent);
        await PostForm(PagePath.OwnBrand, new PageForm(QuestionKey.OwnBrand, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.UnbrandedPackaging, new PageForm(QuestionKey.UnbrandedPackaging, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.ImportingProducts, new PageForm(QuestionKey.ImportingProducts, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.SupplyingEmptyPackaging, new PageForm(QuestionKey.SellingEmptyPackaging, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.HiringLoaning, new PageForm(QuestionKey.HiringLoaning, YesNo.Yes).FormUrlEncodedContent);
        var page = await PostForm(PagePath.OnlineMarketplace, new PageForm(QuestionKey.OnlineMarketplace, YesNo.Yes).FormUrlEncodedContent);
        var pageContent = await page.Content.ReadAsStringAsync();

        pageContent.Should().Contain("<title>Do you supply filled packaging to end users, including businesses? - GOV.UK</title>");
        pageContent.Should().Contain("An end user is the last person to use packaging before discarding it. This could be an individual or a business.");
        pageContent.Should().NotContain("This refers to any organisations that supply filled packaging directly to individual customers in the UK, where they are the end user of the packaging.");
        pageContent.Should().NotContain("<title>Supplying filled packaging to individual customers - GOV.UK</title>");
        pageContent.Should().NotContain("It does not refer to filled packaging supplied to businesses.");
        pageContent.Should().NotContain("Do you supply filled packaging to individual customers?");
    }

    [TestMethod]
    public async Task PlaceDrinksOnMarket_PageContents_AreUpdated()
    {
        await PostForm(PagePath.TypeOfOrganisation, new PageForm(TypeOfOrganisation.IndividualCompany).FormUrlEncodedContent);
        await PostForm(PagePath.AnnualTurnover, new PageForm(AnnualTurnover.OverTwoMillion).FormUrlEncodedContent);
        await PostForm(PagePath.OwnBrand, new PageForm(QuestionKey.OwnBrand, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.UnbrandedPackaging, new PageForm(QuestionKey.UnbrandedPackaging, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.ImportingProducts, new PageForm(QuestionKey.ImportingProducts, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.SupplyingEmptyPackaging, new PageForm(QuestionKey.SellingEmptyPackaging, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.HiringLoaning, new PageForm(QuestionKey.HiringLoaning, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.OnlineMarketplace, new PageForm(QuestionKey.OnlineMarketplace, YesNo.Yes).FormUrlEncodedContent);
        var page = await PostForm(PagePath.SupplyingFilledPackaging, new PageForm(QuestionKey.SupplyingFilledPackaging, YesNo.Yes).FormUrlEncodedContent);
        var pageContent = await page.Content.ReadAsStringAsync();

        pageContent.Should().Contain("Do you supply drinks in single-use containers in the UK?");
        pageContent.Should().NotContain("From October 2025, Scotland will operate a Deposit Return Scheme.".ApostropheToHex());
        pageContent.Should().NotContain("This may affect organisations that place drinks on the market in Scotland.");
        pageContent.Should().NotContain("Find out more about Scotland's Deposit Return Scheme (opens in new tab)");
        pageContent.Should().NotContain("From August 2023, Scotland will operate a Deposit Return Scheme.");
    }

    [TestMethod]
    public async Task MaterialsForDrinksContainers_PageContents_AreUpdated()
    {
        await PostForm(PagePath.TypeOfOrganisation, new PageForm(TypeOfOrganisation.IndividualCompany).FormUrlEncodedContent);
        await PostForm(PagePath.AnnualTurnover, new PageForm(AnnualTurnover.OverTwoMillion).FormUrlEncodedContent);
        await PostForm(PagePath.AmountYouSupply, new PageForm(AmountYouSupply.Handle50TonnesOrMore).FormUrlEncodedContent);
        await PostForm(PagePath.OwnBrand, new PageForm(QuestionKey.OwnBrand, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.UnbrandedPackaging, new PageForm(QuestionKey.UnbrandedPackaging, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.ImportingProducts, new PageForm(QuestionKey.ImportingProducts, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.SupplyingEmptyPackaging, new PageForm(QuestionKey.SellingEmptyPackaging, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.HiringLoaning, new PageForm(QuestionKey.HiringLoaning, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.OnlineMarketplace, new PageForm(QuestionKey.OnlineMarketplace, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.SupplyingFilledPackaging, new PageForm(QuestionKey.SellingEmptyPackaging, YesNo.Yes).FormUrlEncodedContent);
        var page = await PostForm(PagePath.PlaceDrinksOnMarket, new PageForm(QuestionKey.SingleUseContainersOnMarket, YesNo.Yes).FormUrlEncodedContent);
        var pageContent = await page.Content.ReadAsStringAsync();

        pageContent.Should().NotContain("Your packaging activities");

        pageContent.Should().Contain("Polyethylene terephthalate (PET) plastic bottles");
        pageContent.Should().Contain("Glass bottles");
        pageContent.Should().Contain("Steel cans");
        pageContent.Should().Contain("Aluminium cans");
    }

    [TestMethod]
    public async Task AmountYouSupply_PageContents_AreUpdated()
    {
        await PostForm(PagePath.TypeOfOrganisation, new PageForm(TypeOfOrganisation.IndividualCompany).FormUrlEncodedContent);

        var page = await PostForm(PagePath.AnnualTurnover, new PageForm(AnnualTurnover.OverTwoMillion).FormUrlEncodedContent);
        var pageContent = await page.Content.ReadAsStringAsync();

        pageContent.Should().Contain("How much packaging in total do you handle and supply?");

        pageContent.Should().NotContain("Amount of packaging");
        pageContent.Should().NotContain("The amount of packaging you supply or import");
        pageContent.Should().NotContain("You should calculate the total amount of packaging handled or supplied in these ways across your organisation and its subsidiaries.");
        pageContent.Should().NotContain("How much packaging in total did you supply or import in these ways, in 2022?".ApostropheToHex());
    }

    [TestMethod]
    public async Task NoActionsNeeded_AmountYouSupply_PageContents_Test()
    {
        await PostForm(PagePath.TypeOfOrganisation, new PageForm(TypeOfOrganisation.IndividualCompany).FormUrlEncodedContent);
        await PostForm(PagePath.AnnualTurnover, new PageForm(AnnualTurnover.OverTwoMillion).FormUrlEncodedContent);

        var page = await PostForm(PagePath.AmountYouSupply, new PageForm(AmountYouSupply.HandleUnder25Tonnes).FormUrlEncodedContent);
        var pageContent = await page.Content.ReadAsStringAsync();

        pageContent.Should().Contain("This is because your organisation will handle or supply less than 25 tonnes of packaging in 2024.");
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

        pageContent.Should().Contain("What this means for your organisation");
        pageContent.Should().Contain("Nation data is information about which nation in the UK packaging is supplied in and which nation in the UK packaging is discarded in.");

        pageContent.Should().NotContain("A PRN or PERN is evidence that packaging waste has been recycled");
        pageContent.Should().NotContain("a charge to the environmental regulator");
        pageContent.Should().NotContain("importing products in packaging which are supplied or discarded in the UK");
        pageContent.Should().NotContain("supplying empty packaging to businesses that aren't classed as a large organisation".ApostropheToHex());
    }
}