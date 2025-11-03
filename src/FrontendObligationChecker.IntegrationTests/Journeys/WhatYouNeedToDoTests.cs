using FluentAssertions;

using FrontendObligationChecker.Models.ObligationChecker;
using FrontendObligationChecker.UnitTests.Helpers;

using YesNo = FrontendObligationChecker.UnitTests.Helpers.YesNo;

namespace FrontendObligationChecker.IntegrationTests.Journeys;

[TestClass]
public class WhatYouNeedToDoTests : TestBase
{
    [TestMethod]
    public async Task
        OnWhatYouNeedToDoPage_WhenOnAllPagesMaximumOrYesAnswersWereGiven_ThenThePageShowsNonErrorContent()
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

        var redirectToWhatYouNeedToDo = await PostForm(PagePath.AmountYouSupply, new PageForm(AmountYouSupply.Handle50TonnesOrMore).FormUrlEncodedContent);

        var content = await redirectToWhatYouNeedToDo.Content.ReadAsStringAsync(CancellationToken.None);

        redirectToWhatYouNeedToDo.Should().BeSuccessful();
        content.Should().Contain("<title>You will need to take action under the EPR - GOV.UK</title>");
    }
}