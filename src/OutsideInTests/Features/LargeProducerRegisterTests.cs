namespace OutsideInTests.Features;

using AwesomeAssertions;
using AwesomeAssertions.Execution;

using Infrastructure;
using PageModels;

using FrontendObligationChecker.Models.BlobReader;

[Collection(SequentialCollection.Sequential)]
public class LargeProducerRegisterTests : IntegrationTestBase
{
    [Fact]
    public async Task LargeProducers_ShowsPageWithFileList()
    {
        // Arrange - blobs in two year directories with the "large_producers" prefix
        // that LargeProducerRegisterService looks for via IBlobReader
        BlobReader.Blobs =
        [
            new BlobModel
            {
                Name = "2025/large_producers_20250615.csv",
                ContentLength = 1258291,
                CreatedOn = new DateTime(2025, 6, 15)
            },
            new BlobModel
            {
                Name = "2024/large_producers_20241201.csv",
                ContentLength = 921600,
                CreatedOn = new DateTime(2024, 12, 1)
            }
        ];

        // Act
        var page = await GetAsPageModel<LargeProducerRegisterPageModel>("/large-producers");

        // Assert
        using (new AssertionScope())
        {
            page.Heading.Should().NotBeNullOrEmpty();
            page.Files.Should().HaveCount(2);
            page.Files[0].Year.Should().Be("2025");
            page.Files[0].Href.Should().Contain("reportingYear=2025");
            page.Files[1].Year.Should().Be("2024");
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
}
