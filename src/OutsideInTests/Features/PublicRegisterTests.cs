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
    public async Task PublicRegister_ShowsCurrentYearAndNextYearFiles()
    {
        // Arrange - two years of producer register files, matching production behaviour
        // where the synapse pipeline generates a CSV per year that has registrations.
        // Factory sets FakeDateTimeUtcNow to 2025-12-08 (after "11-01" threshold)
        // so the next year file logic activates.
        BlobStorage.ProducerBlobModels = new Dictionary<string, PublicRegisterBlobModel>
        {
            ["2025"] = new PublicRegisterBlobModel
            {
                Name = "2025/Public_Register_Producers_27_November_2025.csv",
                LastModified = new DateTime(2025, 9, 30, 23, 59, 59),
                ContentLength = "132629",
                FileType = "CSV"
            },
            ["2026"] = new PublicRegisterBlobModel
            {
                Name = "2026/Public_Register_Producers_27_November_2025.csv",
                LastModified = new DateTime(2025, 12, 7, 23, 59, 59),
                ContentLength = "25883",
                FileType = "CSV"
            }
        };

        // Act
        var page = await GetAsPageModel<PublicRegisterGuidancePageModel>("/public-register");

        // Assert
        using (new AssertionScope())
        {
            page.Heading.Should().NotBeNullOrEmpty();
            page.DownloadLinks.Should().HaveCount(2, "both current year and next year files should be shown");
            page.DownloadHrefs.Should().Contain(href => href.Contains("fileName=2025"));
            page.DownloadHrefs.Should().Contain(href => href.Contains("fileName=2026"));
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
