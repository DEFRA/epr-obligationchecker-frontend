using FluentAssertions;

using FrontendObligationChecker.Models.ObligationChecker;
using FrontendObligationChecker.UnitTests.Helpers;

using YesNo = FrontendObligationChecker.UnitTests.Helpers.YesNo;

namespace FrontendObligationChecker.IntegrationTests.Journeys;

[TestClass]
public class AmountYouSupplyTests : TestBase
{
    [TestMethod]
    public async Task OnAmountYouSupplyPage_WhenSingleActivityWasSelected_ThenShowSingularText()
    {
        await PostForm(PagePath.TypeOfOrganisation, new PageForm(TypeOfOrganisation.IndividualCompany).FormUrlEncodedContent);
        var redirectToAmountYouSupply = await PostForm(PagePath.AnnualTurnover, new PageForm(AnnualTurnover.OverTwoMillion).FormUrlEncodedContent);

        var content = await redirectToAmountYouSupply.Content.ReadAsStringAsync();

        redirectToAmountYouSupply.Should().BeSuccessful();
        content.Should().Contain("<title>How much packaging in total do you supply? - GOV.UK</title>");
        content.Should().NotContain("Whether you're obligated under EPR for packaging depends on how much packaging you supplied or imported when you:");
        content.Should().NotContain("How much packaging in total did you supply or import in this way, in 2022?");
    }

    [TestMethod]
    public async Task OnAmountYouSupplyPage_WhenMultipleActivitiesWereSelected_ThenShowPluralText()
    {
        await PostForm(PagePath.TypeOfOrganisation, new PageForm(TypeOfOrganisation.IndividualCompany).FormUrlEncodedContent);
        var redirectToAmountYouSupply = await PostForm(PagePath.AnnualTurnover, new PageForm(AnnualTurnover.OverTwoMillion).FormUrlEncodedContent);

        var content = await redirectToAmountYouSupply.Content.ReadAsStringAsync();

        redirectToAmountYouSupply.Should().BeSuccessful();
        content.Should().Contain("<title>How much packaging in total do you supply? - GOV.UK</title>");
        content.Should().NotContain("Some of the ways you supply or import packaging may make you legally obligated under EPR for packaging. These include:");
        content.Should().NotContain("How much packaging in total did you supply or import in these ways, in 2022?");
    }
}