using Microsoft.AspNetCore.Mvc.Testing;

namespace FrontendObligationChecker.IntegrationTests;

public class TestBase
{
    protected readonly HttpClient _httpClient;

    protected TestBase()
    {
        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("https_port", "8000");
                builder.UseSetting("ByPassSessionValidation", "true");
            });

        _httpClient = application.CreateClient();
    }

    protected async Task<HttpResponseMessage> PostForm(string pagePath, FormUrlEncodedContent existingContent)
    {
        var token = await GetAntiForgeryToken($"/ObligationChecker/{pagePath}");

        // Extract the existing key-value pairs
        var keyValuePairs = new List<KeyValuePair<string, string>>(existingContent.ReadAsStringAsync().Result.Split('&').Select(pair =>
        {
            var parts = pair.Split('=');
            return new KeyValuePair<string, string>(parts[0], parts[1]);
        }));

        // Add the new key-value pair
        keyValuePairs.Add(new KeyValuePair<string, string>("__RequestVerificationToken", token));

        var updatedContent = new FormUrlEncodedContent(keyValuePairs);

        return await _httpClient.PostAsync($"/ObligationChecker/{pagePath}", updatedContent);
    }

    protected async Task<string> GetAntiForgeryToken(string path)
    {
        var initialResponse = await _httpClient.GetAsync(path);
        initialResponse.EnsureSuccessStatusCode();

        var htmlDoc = new HtmlAgilityPack.HtmlDocument();
        htmlDoc.LoadHtml(await initialResponse.Content.ReadAsStringAsync());
        return htmlDoc.DocumentNode.SelectSingleNode("//input[@name='__RequestVerificationToken']").Attributes["value"].Value.ToString();
    }
}