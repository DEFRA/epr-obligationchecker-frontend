namespace OutsideInTests.Features;

using AwesomeAssertions;
using AwesomeAssertions.Execution;

using Infrastructure;
using PageModels;

using FrontendObligationChecker.Models.BlobReader;

[Collection(SequentialCollection.Sequential)]
public class PublicRegisterTests : IntegrationTestBase
{
    private const string ProducerContainer = "public-register-producers";

    public override Task InitializeAsync()
    {
        var result = base.InitializeAsync();

        // These MM-DD thresholds control which year folders are requested.
        // "11-01" = next year file shown from 1 Nov onwards
        // "02-01" = previous year file shown until 1 Feb
        ConfigOverrides["PublicRegister:PublicRegisterNextYearStartMonthAndDay"] = "11-01";
        ConfigOverrides["PublicRegister:PublicRegisterPreviousYearEndMonthAndDay"] = "02-01";
        ConfigOverrides["PublicRegister:PublicRegisterBlobContainerName"] = ProducerContainer;

        return result;
    }

    [Fact]
    public async Task ShowsCurrentYearAndNextYearFiles()
    {
        // Arrange
        ConfigOverrides["FeatureManagement:PublicRegisterNextYearEnabled"] = "true";
        ConfigOverrides["PublicRegister:FakeDateTimeUtcNow"] = "2025-12-08"; // after "11-01" threshold

        BlobReader.ContainerBlobs[ProducerContainer] =
        [
            new BlobModel
            {
                Name = "2025/Public_Register_Producers_27_November_2025.csv",
                ContentLength = 132629,
                LastModified = new DateTime(2025, 9, 30, 23, 59, 59)
            },
            new BlobModel
            {
                Name = "2026/Public_Register_Producers_27_November_2025.csv",
                ContentLength = 25883,
                LastModified = new DateTime(2025, 12, 7, 23, 59, 59)
            }
        ];

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
    public async Task ShowsOnlyCurrentYearFile_WhenNextYearNotInBlobStorage()
    {
        // Arrange
        ConfigOverrides["FeatureManagement:PublicRegisterNextYearEnabled"] = "true";
        ConfigOverrides["PublicRegister:FakeDateTimeUtcNow"] = "2025-12-08"; // after "11-01" threshold

        BlobReader.ContainerBlobs[ProducerContainer] =
        [
            new BlobModel
            {
                Name = "2025/Public_Register_Producers_27_November_2025.csv",
                ContentLength = 132629,
                LastModified = new DateTime(2025, 9, 30, 23, 59, 59)
            }
        ];

        // Act
        var page = await GetAsPageModel<PublicRegisterGuidancePageModel>("/public-register");

        // Assert
        using (new AssertionScope())
        {
            page.DownloadLinks.Should().HaveCount(1, "only current year file exists in blob storage");
            page.DownloadHrefs.Should().Contain(href => href.Contains("fileName=2025"));
        }
    }

    [Fact]
    public async Task ShowsOnlyCurrentYearFile_WhenNextYearFeatureFlagOff()
    {
        // Arrange
        ConfigOverrides["FeatureManagement:PublicRegisterNextYearEnabled"] = "false";
        ConfigOverrides["PublicRegister:FakeDateTimeUtcNow"] = "2025-12-08"; // after "11-01" threshold

        BlobReader.ContainerBlobs[ProducerContainer] =
        [
            new BlobModel
            {
                Name = "2025/Public_Register_Producers_27_November_2025.csv",
                ContentLength = 132629,
                LastModified = new DateTime(2025, 9, 30, 23, 59, 59)
            },
            new BlobModel
            {
                Name = "2026/Public_Register_Producers_27_November_2025.csv",
                ContentLength = 25883,
                LastModified = new DateTime(2025, 12, 7, 23, 59, 59)
            }
        ];

        // Act
        var page = await GetAsPageModel<PublicRegisterGuidancePageModel>("/public-register");

        // Assert
        using (new AssertionScope())
        {
            page.DownloadLinks.Should().HaveCount(1, "next year feature flag is off");
            page.DownloadHrefs.Should().Contain(href => href.Contains("fileName=2025"));
            page.DownloadHrefs.Should().NotContain(href => href.Contains("fileName=2026"));
        }
    }

    [Fact]
    public async Task ShowsOnlyCurrentYearFile_WhenDateBeforeNextYearThreshold()
    {
        // Arrange
        ConfigOverrides["FeatureManagement:PublicRegisterNextYearEnabled"] = "true";
        ConfigOverrides["PublicRegister:FakeDateTimeUtcNow"] = "2025-06-01"; // before "11-01" threshold

        BlobReader.ContainerBlobs[ProducerContainer] =
        [
            new BlobModel
            {
                Name = "2025/Public_Register_Producers_01_June_2025.csv",
                ContentLength = 132629,
                LastModified = new DateTime(2025, 6, 1, 23, 59, 59)
            },
            new BlobModel
            {
                Name = "2026/Public_Register_Producers_01_June_2025.csv",
                ContentLength = 25883,
                LastModified = new DateTime(2025, 6, 1, 23, 59, 59)
            }
        ];

        // Act
        var page = await GetAsPageModel<PublicRegisterGuidancePageModel>("/public-register");

        // Assert
        using (new AssertionScope())
        {
            page.DownloadLinks.Should().HaveCount(1, "date is before 11-01 threshold");
            page.DownloadHrefs.Should().Contain(href => href.Contains("fileName=2025"));
            page.DownloadHrefs.Should().NotContain(href => href.Contains("fileName=2026"));
        }
    }

    [Fact]
    public async Task ShowsGuidancePage_WithNoFiles()
    {
        // Arrange
        ConfigOverrides["FeatureManagement:PublicRegisterNextYearEnabled"] = "false";
        ConfigOverrides["PublicRegister:FakeDateTimeUtcNow"] = "2025-12-08";

        // no blob data configured

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
