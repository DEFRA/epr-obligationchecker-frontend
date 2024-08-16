using FluentAssertions;

using FrontendObligationChecker.Models.ObligationChecker;

namespace FrontendObligationChecker.IntegrationTests.Journeys;

[TestClass]
public class StartPageTests : TestBase
{
    [TestMethod]
    public async Task OnStartPage_OnPageLoad_CheckTitleText()
    {
        var response = await _httpClient.GetAsync($"/ObligationChecker/{PagePath.StartPage}");
        var content = await response.Content.ReadAsStringAsync();

        response.Should().BeSuccessful();
        content.Should().Contain("Check if you need to report packaging data");
        content.Should().NotContain("There is a problem");
    }

    [TestMethod]
    public async Task OnStartPage_OnPageLoad_TheNextPageIsTypeOfOrganisation()
    {
        var tokenValue = await GetAntiForgeryToken($"/ObligationChecker/{PagePath.TypeOfOrganisation}");

        var response = await _httpClient.PostAsync(
        $"/ObligationChecker/{PagePath.StartPage}",
        new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
        {
            new ("__RequestVerificationToken", tokenValue),
            new(QuestionKey.TypeOfOrganisation, "parent")
        }));

        var content = await response.Content.ReadAsStringAsync();

        response.Should().BeSuccessful();
        content.Should().Contain("What type of organisation are you?");
    }
}