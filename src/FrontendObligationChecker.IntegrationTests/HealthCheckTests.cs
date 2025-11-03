using FluentAssertions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FrontendObligationChecker.IntegrationTests;
[TestClass]
public class HealthCheckTests : TestBase
{
    [TestMethod]
    public async Task HealthCheck_ReturnsOk()
    {
        // Arrange
        const string url = "/admin/health";

        // Act
        var response = await _httpClient.GetAsync(url, CancellationToken.None);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.Should().BeSuccessful();
        content.Should().Be(HealthStatus.Healthy.ToString());
    }
}