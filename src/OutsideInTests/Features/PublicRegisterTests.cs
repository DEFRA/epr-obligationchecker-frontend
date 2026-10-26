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

        ConfigOverrides["PublicRegister:PublicRegisterBlobContainerName"] = ProducerContainer;

        return result;
    }

    [Fact]
    public async Task ShowsCurrentYearAndNextYearFiles()
    {
        // Arrange
        ConfigOverrides["FeatureManagement:PublicRegisterNextYearEnabled"] = "true";
        ConfigOverrides["PublicRegister:FakeDateTimeUtcNow"] = "2025-12-08";

        BlobReader.ContainerBlobs[ProducerContainer] =
        [
            new BlobModel
            {
                Name = "2025/2025_register_of_producers.csv",
                ContentLength = 132629,
                LastModified = new DateTime(2025, 9, 30, 23, 59, 59)
            },
            new BlobModel
            {
                Name = "2026/2026_register_of_producers.csv",
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
        ConfigOverrides["PublicRegister:FakeDateTimeUtcNow"] = "2025-12-08";

        BlobReader.ContainerBlobs[ProducerContainer] =
        [
            new BlobModel
            {
                Name = "2025/2025_register_of_producers.csv",
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
        ConfigOverrides["PublicRegister:FakeDateTimeUtcNow"] = "2025-12-08";

        BlobReader.ContainerBlobs[ProducerContainer] =
        [
            new BlobModel
            {
                Name = "2025/2025_register_of_producers.csv",
                ContentLength = 132629,
                LastModified = new DateTime(2025, 9, 30, 23, 59, 59)
            },
            new BlobModel
            {
                Name = "2026/2026_register_of_producers.csv",
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
    public async Task ShowsCurrentYearAndNextYearFiles_WhenFlagOnRegardlessOfDate()
    {
        // Arrange
        ConfigOverrides["FeatureManagement:PublicRegisterNextYearEnabled"] = "true";
        ConfigOverrides["PublicRegister:FakeDateTimeUtcNow"] = "2025-06-01"; // before old "11-01" threshold — should still show next year

        BlobReader.ContainerBlobs[ProducerContainer] =
        [
            new BlobModel
            {
                Name = "2025/2025_register_of_producers.csv",
                ContentLength = 132629,
                LastModified = new DateTime(2025, 6, 1, 23, 59, 59)
            },
            new BlobModel
            {
                Name = "2026/2026_register_of_producers.csv",
                ContentLength = 25883,
                LastModified = new DateTime(2025, 6, 1, 23, 59, 59)
            }
        ];

        // Act
        var page = await GetAsPageModel<PublicRegisterGuidancePageModel>("/public-register");

        // Assert
        using (new AssertionScope())
        {
            page.DownloadLinks.Should().HaveCount(2, "feature flag alone controls next year — no date threshold");
            page.DownloadHrefs.Should().Contain(href => href.Contains("fileName=2025"));
            page.DownloadHrefs.Should().Contain(href => href.Contains("fileName=2026"));
        }
    }

    [Fact]
    public async Task ShowsPreviousYearFileOnMainRegisterOnly_AfterFirstFebruary_WhenArchivedUpToYearBlank()
    {
        // Arrange — blank setting: nothing is archived, so 2026 stays on the main register after 1 Feb
        // and is not on the archive page
        ConfigOverrides["FeatureManagement:PublicRegisterNextYearEnabled"] = "false";
        ConfigOverrides["PublicRegister:FakeDateTimeUtcNow"] = "2027-02-01";
        ConfigOverrides["PublicRegister:ArchivedUpToYear"] = "";

        BlobReader.ContainerBlobs[ProducerContainer] =
        [
            new BlobModel
            {
                Name = "2026/2026_register_of_producers.csv",
                ContentLength = 132629,
                LastModified = new DateTime(2027, 1, 31, 23, 59, 59)
            },
            new BlobModel
            {
                Name = "2027/2027_register_of_producers.csv",
                ContentLength = 25883,
                LastModified = new DateTime(2027, 1, 31, 23, 59, 59)
            }
        ];

        // Act
        var page = await GetAsPageModel<PublicRegisterGuidancePageModel>("/public-register");
        var archivePage = await GetAsPageModel<LargeProducerRegisterPageModel>("/large-producers");

        // Assert
        using (new AssertionScope())
        {
            page.DownloadLinks.Should().HaveCount(2);
            page.DownloadHrefs.Should().Contain(href => href.Contains("fileName=2026"));
            page.DownloadHrefs.Should().Contain(href => href.Contains("fileName=2027"));
            archivePage.RegisterOfProducersLinks.Should().BeEmpty();
        }
    }

    [Fact]
    public async Task ShowsArchivedYearsOnArchivePageOnly_WhenArchivedUpToYearSet()
    {
        // Arrange — setting 2026: 2025 and 2026 are on the archive page, 2027 stays on the main register
        ConfigOverrides["FeatureManagement:PublicRegisterNextYearEnabled"] = "false";
        ConfigOverrides["PublicRegister:FakeDateTimeUtcNow"] = "2027-02-01";
        ConfigOverrides["PublicRegister:ArchivedUpToYear"] = "2026";

        BlobReader.ContainerBlobs[ProducerContainer] =
        [
            new BlobModel
            {
                Name = "2025/2025_register_of_producers.csv",
                ContentLength = 100000,
                LastModified = new DateTime(2025, 12, 1)
            },
            new BlobModel
            {
                Name = "2026/2026_register_of_producers.csv",
                ContentLength = 200000,
                LastModified = new DateTime(2026, 12, 1)
            },
            new BlobModel
            {
                Name = "2027/2027_register_of_producers.csv",
                ContentLength = 300000,
                LastModified = new DateTime(2027, 1, 31)
            }
        ];

        // Act
        var page = await GetAsPageModel<PublicRegisterGuidancePageModel>("/public-register");
        var archivePage = await GetAsPageModel<LargeProducerRegisterPageModel>("/large-producers");

        // Assert
        using (new AssertionScope())
        {
            page.DownloadLinks.Should().HaveCount(1);
            page.DownloadHrefs.Should().ContainSingle(href => href.Contains("fileName=2027"));
            archivePage.RegisterOfProducersLinks.Should().HaveCount(2);
            archivePage.RegisterOfProducersLinks[0].Href.Should().Contain("fileName=2026");
            archivePage.RegisterOfProducersLinks[1].Href.Should().Contain("fileName=2025");
        }
    }

    [Fact]
    public async Task FailsToStart_WhenArchivedUpToYearInvalid()
    {
        // Arrange
        ConfigOverrides["PublicRegister:ArchivedUpToYear"] = "2O26";

        // Act
        var act = () => Client.GetAsync("/public-register");

        // Assert
        (await act.Should().ThrowAsync<InvalidOperationException>())
            .WithMessage("*PublicRegister:ArchivedUpToYear*");
    }

    [Fact]
    public async Task LinksToRegisterOfProducersFile_WhenNewerDebugAndGeneratedFilesExist()
    {
        // Arrange
        ConfigOverrides["FeatureManagement:PublicRegisterNextYearEnabled"] = "false";
        ConfigOverrides["PublicRegister:FakeDateTimeUtcNow"] = "2026-10-22";

        BlobReader.ContainerBlobs[ProducerContainer] =
        [
            new BlobModel
            {
                Name = "2026/2026_register_of_producers.csv",
                ContentLength = 132629,
                LastModified = new DateTime(2026, 10, 1, 23, 59, 59)
            },
            new BlobModel
            {
                Name = "2026/2026_register_of_producers_DEBUG.csv",
                ContentLength = 265258,
                LastModified = new DateTime(2026, 10, 21, 23, 59, 59)
            },
            new BlobModel
            {
                Name = "2026/2026_register_of_producers_DEBUG_generated_on_2026-10-21.csv",
                ContentLength = 265258,
                LastModified = new DateTime(2026, 10, 21, 23, 59, 59)
            },
            new BlobModel
            {
                Name = "2026/2026_register_of_producers_generated_on_2026-10-16.csv",
                ContentLength = 132629,
                LastModified = new DateTime(2026, 10, 16, 23, 59, 59)
            }
        ];

        // Act
        var page = await GetAsPageModel<PublicRegisterGuidancePageModel>("/public-register");

        // Assert
        using (new AssertionScope())
        {
            page.DownloadLinks.Should().HaveCount(1);
            page.DownloadHrefs.Should().ContainSingle(href => href.Contains("fileName=2026/2026_register_of_producers.csv&"));
        }
    }

    [Fact]
    public async Task DownloadsRegisterOfProducersFile()
    {
        // Arrange
        BlobReader.FileContents["2026/2026_register_of_producers.csv"] = "register"u8.ToArray();

        // Act
        var response = await Client.GetAsync("/public-register/report?fileName=2026/2026_register_of_producers.csv&type=public-register-producers");

        // Assert
        using (new AssertionScope())
        {
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            response.Content.Headers.ContentType?.MediaType.Should().Be("text/csv");
            (await response.Content.ReadAsStringAsync()).Should().Be("register");
        }
    }

    [Theory]
    [InlineData("2026/2026_register_of_producers_DEBUG.csv")]
    [InlineData("2026/2026_register_of_producers_DEBUG_generated_on_2026-10-21.csv")]
    [InlineData("2026/2026_register_of_producers_generated_on_2026-10-16.csv")]
    public async Task DoesNotDownloadDebugOrGeneratedFile(string fileName)
    {
        // Arrange
        BlobReader.FileContents[fileName] = "internal"u8.ToArray();

        // Act
        var response = await Client.GetAsync($"/public-register/report?fileName={fileName}&type=public-register-producers");

        // Assert
        using (new AssertionScope())
        {
            response.RequestMessage?.RequestUri?.AbsolutePath.Should().Be("/public-register/file-not-downloaded");
            response.Content.Headers.ContentType?.MediaType.Should().Be("text/html");
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
