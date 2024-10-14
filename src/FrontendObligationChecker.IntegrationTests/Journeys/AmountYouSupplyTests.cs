using FluentAssertions;

using FrontendObligationChecker.Models.ObligationChecker;
using FrontendObligationChecker.UnitTests.Helpers;

using YesNo = FrontendObligationChecker.UnitTests.Helpers.YesNo;

namespace FrontendObligationChecker.IntegrationTests.Journeys;

[TestClass]
public class AmountYouSupplyTests : TestBase
{
    // -- TODO -- Will revisit this.

    /*
    [TestMethod]
    public async Task OnAmountYouSupplyPage_WhenSingleActivityWasSelected_ThenShowSingularText()
    {
        await PostForm(PagePath.TypeOfOrganisation, new PageForm(TypeOfOrganisation.IndividualCompany).FormUrlEncodedContent);
        await PostForm(PagePath.AnnualTurnover, new PageForm(AnnualTurnover.OverTwoMillion).FormUrlEncodedContent);
        await PostForm(PagePath.OwnBrand, new PageForm(QuestionKey.OwnBrand, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.UnbrandedPackaging, new PageForm(QuestionKey.UnbrandedPackaging, YesNo.No).FormUrlEncodedContent);
        await PostForm(PagePath.ImportingProducts, new PageForm(QuestionKey.ImportingProducts, YesNo.No).FormUrlEncodedContent);
        await PostForm(PagePath.SupplyingEmptyPackaging, new PageForm(QuestionKey.SellingEmptyPackaging, YesNo.No).FormUrlEncodedContent);
        await PostForm(PagePath.HiringLoaning, new PageForm(QuestionKey.HiringLoaning, YesNo.No).FormUrlEncodedContent);
        await PostForm(PagePath.OnlineMarketplace, new PageForm(QuestionKey.OnlineMarketplace, YesNo.No).FormUrlEncodedContent);
        await PostForm(PagePath.SupplyingFilledPackaging, new PageForm(QuestionKey.SellingEmptyPackaging, YesNo.No).FormUrlEncodedContent);

        var redirectToAmountYouSupply = await PostForm(PagePath.PlaceDrinksOnMarket, new PageForm(QuestionKey.SingleUseContainersOnMarket, YesNo.No).FormUrlEncodedContent);

        var content = await redirectToAmountYouSupply.Content.ReadAsStringAsync();

        redirectToAmountYouSupply.Should().BeSuccessful();
        content.Should().Contain("<title>The amount of packaging you supply or import - GOV.UK</title>");
        content.Should().Contain("Whether you're obligated under EPR for packaging depends on how much packaging you supplied or imported when you:");
        content.Should().Contain("How much packaging in total did you supply or import in this way, in 2022?");
    }
    */

    // -- TODO -- Will revisit this.
    /*
    [TestMethod]
    public async Task OnAmountYouSupplyPage_WhenMultipleActivitiesWereSelected_ThenShowPluralText()
    {
        await PostForm(PagePath.TypeOfOrganisation, new PageForm(TypeOfOrganisation.IndividualCompany).FormUrlEncodedContent);
        await PostForm(PagePath.AnnualTurnover, new PageForm(AnnualTurnover.OverTwoMillion).FormUrlEncodedContent);
        await PostForm(PagePath.OwnBrand, new PageForm(QuestionKey.OwnBrand, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.UnbrandedPackaging, new PageForm(QuestionKey.UnbrandedPackaging, YesNo.Yes).FormUrlEncodedContent);
        await PostForm(PagePath.ImportingProducts, new PageForm(QuestionKey.ImportingProducts, YesNo.No).FormUrlEncodedContent);
        await PostForm(PagePath.SupplyingEmptyPackaging, new PageForm(QuestionKey.SellingEmptyPackaging, YesNo.No).FormUrlEncodedContent);
        await PostForm(PagePath.HiringLoaning, new PageForm(QuestionKey.HiringLoaning, YesNo.No).FormUrlEncodedContent);
        await PostForm(PagePath.OnlineMarketplace, new PageForm(QuestionKey.OnlineMarketplace, YesNo.No).FormUrlEncodedContent);
        await PostForm(PagePath.SupplyingFilledPackaging, new PageForm(QuestionKey.SellingEmptyPackaging, YesNo.No).FormUrlEncodedContent);

        var redirectToAmountYouSupply = await PostForm(PagePath.PlaceDrinksOnMarket, new PageForm(QuestionKey.SingleUseContainersOnMarket, YesNo.No).FormUrlEncodedContent);

        var content = await redirectToAmountYouSupply.Content.ReadAsStringAsync();

        redirectToAmountYouSupply.Should().BeSuccessful();
        content.Should().Contain("<title>The amount of packaging you supply or import - GOV.UK</title>");
        content.Should().Contain("Some of the ways you supply or import packaging may make you legally obligated under EPR for packaging. These include:");
        content.Should().Contain("How much packaging in total did you supply or import in these ways, in 2022?");
    */
}