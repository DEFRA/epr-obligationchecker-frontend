namespace OutsideInTests.Features;

using AwesomeAssertions;
using AwesomeAssertions.Execution;

using Infrastructure;
using PageModels;

using FrontendObligationChecker.Models.BlobReader;

[Collection(SequentialCollection.Sequential)]
public class PublicRegisterTests : IntegrationTestBase
{
    [Fact]
    public async Task PublicRegister_ShowsGuidancePage()
    {
        // Arrange
        var currentYear = DateTime.UtcNow.Year.ToString();
        BlobStorage.ProducerBlobModels = new Dictionary<string, PublicRegisterBlobModel>
        {
            [currentYear] = new PublicRegisterBlobModel
            {
                Name = $"{currentYear}/producers-2025.csv",
                LastModified = new DateTime(2025, 6, 1),
                ContentLength = "12345",
                FileType = "CSV"
            }
        };

        // Act
        var page = await GetAsPageModel<PublicRegisterGuidancePageModel>("/public-register");

        // Assert
        using (new AssertionScope())
        {
            page.Heading.Should().NotBeNullOrEmpty();
            page.DownloadLinks.Should().NotBeEmpty();
            page.DownloadHrefs.Should().Contain(href => href.Contains("/public-register/report"));
        }
    }

    [Fact]
    public async Task PublicRegister_ShowsGuidancePage_WithNoFiles()
    {
        // Arrange - no blob data configured (default empty)

        // Act
        var page = await GetAsPageModel<PublicRegisterGuidancePageModel>("/public-register");

        // Assert
        using (new AssertionScope())
        {
            page.Heading.Should().NotBeNullOrEmpty();
            page.Description.Should().NotBeNullOrEmpty();
        }
    }
}
