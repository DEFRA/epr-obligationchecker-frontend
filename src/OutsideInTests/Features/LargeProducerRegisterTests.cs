namespace OutsideInTests.Features;

using AwesomeAssertions;
using AwesomeAssertions.Execution;

using Infrastructure;
using PageModels;

using FrontendObligationChecker.ViewModels.LargeProducer;

[Collection(SequentialCollection.Sequential)]
public class LargeProducerRegisterTests : IntegrationTestBase
{
    [Fact]
    public async Task LargeProducers_ShowsPageWithFileList()
    {
        // Arrange
        LargeProducerRegister.FileInfoList =
        [
            new LargeProducerFileInfoViewModel
            {
                ReportingYear = 2025,
                DateCreated = new DateTime(2025, 6, 15),
                DisplayFileSize = "1.2 MB"
            },
            new LargeProducerFileInfoViewModel
            {
                ReportingYear = 2024,
                DateCreated = new DateTime(2024, 12, 1),
                DisplayFileSize = "900 KB"
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
        // Arrange - no files configured (default empty list)

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
