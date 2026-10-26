namespace OutsideInTests.Features;

using AwesomeAssertions;
using AwesomeAssertions.Execution;

using Infrastructure;
using PageModels;

using FrontendObligationChecker.Models.BlobReader;

[Collection(SequentialCollection.Sequential)]
public class LargeProducerRegisterTests : IntegrationTestBase
{
    private const string ProducerContainer = "public-register-producers";

    public override Task InitializeAsync()
    {
        var result = base.InitializeAsync();

        ConfigOverrides["PublicRegister:PublicRegisterBlobContainerName"] = ProducerContainer;

        return result;
    }

    [Fact]
    public async Task LargeProducers_ShowsPageWithFileList()
    {
        // Arrange - blobs in two year directories with the "large_producers" prefix
        // that LargeProducerRegisterService looks for via IBlobReader.
        // Only years < 2025 appear in the "List of Large Producers" section.
        BlobReader.Blobs =
        [
            new BlobModel
            {
                Name = "2024/large_producers_20241201.csv",
                ContentLength = 921600,
                CreatedOn = new DateTime(2024, 12, 1)
            },
            new BlobModel
            {
                Name = "2023/large_producers_20231201.csv",
                ContentLength = 800000,
                CreatedOn = new DateTime(2023, 12, 1)
            }
        ];

        // Act
        var page = await GetAsPageModel<LargeProducerRegisterPageModel>("/large-producers");

        // Assert
        using (new AssertionScope())
        {
            page.Heading.Should().NotBeNullOrEmpty();
            page.Files.Should().HaveCount(2);
            page.Files[0].Href.Should().Contain("reportingYear=2024");
            page.Files[1].Href.Should().Contain("reportingYear=2023");
        }
    }

    [Fact]
    public async Task LargeProducers_ShowsPageWithNoFiles_WhenNoneAvailable()
    {
        // Arrange - no blobs configured (default empty list)

        // Act
        var page = await GetAsPageModel<LargeProducerRegisterPageModel>("/large-producers");

        // Assert
        using (new AssertionScope())
        {
            page.Heading.Should().NotBeNullOrEmpty();
            page.Files.Should().BeEmpty();
        }
    }

    [Fact]
    public async Task RegisterOfProducers_ExcludesPreviousYear_WhenArchivedUpToYearNotUpdated()
    {
        // Arrange — February 1 2027 with setting 2025: previous year (2026) stays on the main
        // register until the setting is updated, so the archive should show only 2025.
        ConfigOverrides["PublicRegister:FakeDateTimeUtcNow"] = "2027-02-01";
        ConfigOverrides["PublicRegister:ArchivedUpToYear"] = "2025";

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
            }
        ];

        // Act
        var page = await GetAsPageModel<LargeProducerRegisterPageModel>("/large-producers");

        // Assert — only 2025 should appear, not 2026
        using (new AssertionScope())
        {
            page.RegisterOfProducersLinks.Should().HaveCount(1);
            page.RegisterOfProducersLinks[0].Href.Should().Contain("fileName=2025");
        }
    }

    [Fact]
    public async Task RegisterOfProducers_ShowsNoYears_WhenArchivedUpToYearBlank()
    {
        // Arrange — February 1 2027 with a blank setting: nothing is archived.
        ConfigOverrides["PublicRegister:FakeDateTimeUtcNow"] = "2027-02-01";
        ConfigOverrides["PublicRegister:ArchivedUpToYear"] = "";

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
            }
        ];

        // Act
        var page = await GetAsPageModel<LargeProducerRegisterPageModel>("/large-producers");

        // Assert
        page.RegisterOfProducersLinks.Should().BeEmpty();
    }

    [Fact]
    public async Task RegisterOfProducers_ShowsYearsUpToArchivedUpToYear()
    {
        // Arrange — January 10 2028 with setting 2026: 2025 and 2026 are archived, 2027 stays
        // on the main register.
        ConfigOverrides["PublicRegister:FakeDateTimeUtcNow"] = "2028-01-10";
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
                LastModified = new DateTime(2027, 12, 1)
            }
        ];

        // Act
        var page = await GetAsPageModel<LargeProducerRegisterPageModel>("/large-producers");

        // Assert — 2026 and 2025 should appear (ordered descending), not 2027
        using (new AssertionScope())
        {
            page.RegisterOfProducersLinks.Should().HaveCount(2);
            page.RegisterOfProducersLinks[0].Href.Should().Contain("fileName=2026");
            page.RegisterOfProducersLinks[1].Href.Should().Contain("fileName=2025");
        }
    }
}
