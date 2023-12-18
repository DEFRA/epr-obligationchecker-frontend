using FluentAssertions;

using FrontendObligationChecker.Models.ObligationChecker;

namespace FrontendObligationChecker.IntegrationTests.Journeys;

[TestClass]
public class TypeOfOrganisationTests : TestBase
{
    [TestMethod]
    public async Task OnTypeOfOrganisationPage_WhenNoSelectionWasMade_ThenShowThereIsAProblemText()
    {
        var response = await _httpClient.GetAsync($"/check-if-you-need-to-report/{PagePath.TypeOfOrganisation}");
        var content = await response.Content.ReadAsStringAsync();

        response.Should().BeSuccessful();
        content.Should().Contain("<title>What type of organisation are you? - GOV.UK</title>");
        content.Should().NotContain("There is a problem");
    }

    [TestMethod]
    public async Task OnTypeOfOrganisationPage_WhenEmptyFormWasSubmitted_ThenShowThereIsAProblemText()
    {
        var emptyData = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>());

        var response = await _httpClient.PostAsync($"/check-if-you-need-to-report/{PagePath.TypeOfOrganisation}", emptyData);
        var content = await response.Content.ReadAsStringAsync();

        response.Should().BeSuccessful();
        content.Should().Contain("There is a problem");
    }

    [TestMethod]
    public async Task OnTypeOfOrganisationPage_WhenASelectionWasMade_ThenTheNextPageIsAnnualTurnover()
    {
        var response = await _httpClient.PostAsync(
            $"/check-if-you-need-to-report/{PagePath.TypeOfOrganisation}",
            new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            {
                new(QuestionKey.TypeOfOrganisation, "parent")
            }));

        var content = await response.Content.ReadAsStringAsync();

        response.Should().BeSuccessful();
        content.Should().Contain("<title>What was your group&#x27;s last annual turnover? - GOV.UK</title>");
    }
}