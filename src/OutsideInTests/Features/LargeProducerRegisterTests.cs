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

        ConfigOverrides["PublicRegister:PublicRegisterPreviousYearEndMonthAndDay"] = "02-01";
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
    public async Task RegisterOfProducers_ExcludesPreviousYear_BeforeCutover()
    {
        // Arrange — January 10 2027: previous year (2026) is still on the main register,
        // so the archive should show only 2025.
        ConfigOverrides["PublicRegister:FakeDateTimeUtcNow"] = "2027-01-10";

        BlobReader.ContainerBlobs[ProducerContainer] =
        [
            new BlobModel
            {
                Name = "2025/Public_Register_Producers_01_December_2025.csv",
                ContentLength = 100000,
                LastModified = new DateTime(2025, 12, 1)
            },
            new BlobModel
            {
                Name = "2026/Public_Register_Producers_01_December_2026.csv",
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
    public async Task RegisterOfProducers_IncludesPreviousYear_OnCutoverDay()
    {
        // Arrange — February 1 2027: cutover day. Previous year (2026) is dropped from
        // the main register and should now appear in the archive.
        ConfigOverrides["PublicRegister:FakeDateTimeUtcNow"] = "2027-02-01";

        BlobReader.ContainerBlobs[ProducerContainer] =
        [
            new BlobModel
            {
                Name = "2025/Public_Register_Producers_01_December_2025.csv",
                ContentLength = 100000,
                LastModified = new DateTime(2025, 12, 1)
            },
            new BlobModel
            {
                Name = "2026/Public_Register_Producers_01_December_2026.csv",
                ContentLength = 200000,
                LastModified = new DateTime(2026, 12, 1)
            }
        ];

        // Act
        var page = await GetAsPageModel<LargeProducerRegisterPageModel>("/large-producers");

        // Assert — both 2025 and 2026 should appear (ordered descending)
        using (new AssertionScope())
        {
            page.RegisterOfProducersLinks.Should().HaveCount(2);
            page.RegisterOfProducersLinks[0].Href.Should().Contain("fileName=2026");
            page.RegisterOfProducersLinks[1].Href.Should().Contain("fileName=2025");
        }
    }

    [Fact]
    public async Task RegisterOfProducers_IncludesPreviousYear_OnLeapYearCutoverDay()
    {
        // Arrange — February 1 2028 (leap year): cutover day, previous year (2027) drops
        // from main register and appears in archive.
        ConfigOverrides["PublicRegister:FakeDateTimeUtcNow"] = "2028-02-01";

        BlobReader.ContainerBlobs[ProducerContainer] =
        [
            new BlobModel
            {
                Name = "2025/Public_Register_Producers_01_December_2025.csv",
                ContentLength = 100000,
                LastModified = new DateTime(2025, 12, 1)
            },
            new BlobModel
            {
                Name = "2026/Public_Register_Producers_01_December_2026.csv",
                ContentLength = 200000,
                LastModified = new DateTime(2026, 12, 1)
            },
            new BlobModel
            {
                Name = "2027/Public_Register_Producers_01_December_2027.csv",
                ContentLength = 300000,
                LastModified = new DateTime(2027, 12, 1)
            }
        ];

        // Act
        var page = await GetAsPageModel<LargeProducerRegisterPageModel>("/large-producers");

        // Assert — 2025, 2026, and 2027 should all appear
        using (new AssertionScope())
        {
            page.RegisterOfProducersLinks.Should().HaveCount(3);
            page.RegisterOfProducersLinks[0].Href.Should().Contain("fileName=2027");
            page.RegisterOfProducersLinks[1].Href.Should().Contain("fileName=2026");
            page.RegisterOfProducersLinks[2].Href.Should().Contain("fileName=2025");
        }
    }
}
