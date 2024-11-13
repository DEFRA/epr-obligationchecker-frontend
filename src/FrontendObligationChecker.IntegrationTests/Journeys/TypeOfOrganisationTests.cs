using FluentAssertions;
using FrontendObligationChecker.IntegrationTests.Extensions;
using FrontendObligationChecker.Models.ObligationChecker;

namespace FrontendObligationChecker.IntegrationTests.Journeys;

[TestClass]
public class TypeOfOrganisationTests : TestBase
{
    [TestMethod]
    public async Task OnTypeOfOrganisationPage_WhenNoSelectionWasMade_ThenShowThereIsAProblemText()
    {
        var response = await _httpClient.GetAsync($"/ObligationChecker/{PagePath.TypeOfOrganisation}");
        var content = await response.Content.ReadAsStringAsync();

        response.Should().BeSuccessful();
        content.Should().Contain("<title>What type of organisation are you? - GOV.UK</title>");
        content.Should().NotContain("There is a problem");
    }

    [TestMethod]
    public async Task OnTypeOfOrganisationPage_WhenEmptyFormWasSubmitted_ThenShowThereIsAProblemText()
    {
        var tokenValue = await GetAntiForgeryToken($"/ObligationChecker/{PagePath.TypeOfOrganisation}");

        var response = await _httpClient.PostAsync(
        $"/ObligationChecker/{PagePath.TypeOfOrganisation}",
        new FormUrlEncodedContent(new Dictionary<string, string>()
        {
            { "__RequestVerificationToken", tokenValue }
        }));

        var content = await response.Content.ReadAsStringAsync();
        response.Should().BeSuccessful();
        content.Should().Contain("There is a problem");
    }

    [TestMethod]
    public async Task OnTypeOfOrganisationPage_WhenASelectionWasMade_ThenTheNextPageIsAnnualTurnover()
    {
        var tokenValue = await GetAntiForgeryToken($"/ObligationChecker/{PagePath.TypeOfOrganisation}");

        var response = await _httpClient.PostAsync(
        $"/ObligationChecker/{PagePath.TypeOfOrganisation}",
        new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
        {
            new ("__RequestVerificationToken", tokenValue),
            new(QuestionKey.TypeOfOrganisation, "parent")
        }));

        var content = await response.Content.ReadAsStringAsync();

        response.Should().BeSuccessful();
        content.Should().Contain("<title>What was your group’s last annual turnover? - GOV.UK</title>".CurvedApostropheToHex());
    }
}