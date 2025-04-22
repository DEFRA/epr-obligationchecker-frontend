namespace FrontendObligationChecker.UnitTests.Controllers
{
    using System.Globalization;
    using System.Reflection;
    using System.Threading.Tasks;
    using FluentAssertions;
    using FrontendObligationChecker.Controllers;
    using FrontendObligationChecker.Exceptions;
    using FrontendObligationChecker.Models.BlobReader;
    using FrontendObligationChecker.Models.Config;
    using FrontendObligationChecker.Services.PublicRegister;
    using FrontendObligationChecker.ViewModels.PublicRegister;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class PublicRegisterControllerTests
    {
        private Mock<IBlobStorageService> _blobStorageServiceMock = null!;
        private IOptions<PublicRegisterOptions> _publicRegisterOptions = null!;
        private IOptions<ExternalUrlsOptions> _externalUrlsOptions = null!;
        private PublicRegisterController _controller = null!;
        private DateTime _publishedDate;
        private DateTime _lastModified;

        [TestInitialize]
        public void Setup()
        {
            _blobStorageServiceMock = new Mock<IBlobStorageService>();

            _publishedDate = new DateTime(2025, 4, 10, 0, 0, 0, DateTimeKind.Utc);
            _lastModified = new DateTime(2025, 4, 15, 0, 0, 0, DateTimeKind.Utc);

            var producerBlob = new PublicRegisterBlobModel
            {
                Name = "producers.csv",
                PublishedDate = _publishedDate,
                LastModified = _lastModified,
                ContentLength = "115",
                FileType = "text/csv"
            };

            var complianceBlob = new PublicRegisterBlobModel
            {
                Name = "schemes.csv",
                PublishedDate = _publishedDate,
                LastModified = _lastModified,
                ContentLength = "450",
                FileType = "text/csv"
            };

            _blobStorageServiceMock
                .Setup(x => x.GetLatestFilePropertiesAsync("producers-container"))
                .ReturnsAsync(producerBlob);

            _blobStorageServiceMock
                .Setup(x => x.GetLatestFilePropertiesAsync("schemes-container"))
                .ReturnsAsync(complianceBlob);

            _publicRegisterOptions = Options.Create(new PublicRegisterOptions
            {
                PublicRegisterBlobContainerName = "producers-container",
                PublicRegisterCsoBlobContainerName = "schemes-container"
            });

            _externalUrlsOptions = Options.Create(new ExternalUrlsOptions
            {
                DefraUrl = "https://www.defraurl.com"
            });

            _controller = new PublicRegisterController(_blobStorageServiceMock.Object, _externalUrlsOptions, _publicRegisterOptions);
        }

        [TestMethod]
        [DataRow("producers-container")]
        [DataRow("schemes-container")]
        public async Task Guidance_ReturnsExpectedViewWithCorrectModel(string containerName)
        {
            // Act
            var result = await _controller.Get();

            // Assert
            result.Should().BeOfType<ViewResult>();

            var viewResult = result as ViewResult;
            viewResult!.ViewName.Should().Be("Guidance");
            viewResult.Model.Should().BeOfType<GuidanceViewModel>();

            var model = (GuidanceViewModel)viewResult.Model!;

            var expectedDate = _publishedDate.ToString("d MMMM yyyy", CultureInfo.InvariantCulture);
            var expectedLastUpdated = _lastModified.ToString("d MMMM yyyy", CultureInfo.InvariantCulture);

            model.PublishedDate.Should().Be(expectedDate);
            model.LastUpdated.Should().Be(expectedLastUpdated);

            model.ProducerRegisteredFile.Should().NotBeNull();
            model.ProducerRegisteredFile!.FileName.Should().Be("producers.csv");
            model.ProducerRegisteredFile.FileSize.Should().Be("115");
            model.ProducerRegisteredFile.FileType.Should().Be("text/csv");
            model.ProducerRegisteredFile.DatePublished.Should().Be(expectedDate);
            model.ProducerRegisteredFile.DateLastModified.Should().Be(expectedLastUpdated);

            model.ComplianceSchemeRegisteredFile.Should().NotBeNull();
            model.ComplianceSchemeRegisteredFile!.FileName.Should().Be("schemes.csv");
            model.ComplianceSchemeRegisteredFile.FileSize.Should().Be("450");
            model.ComplianceSchemeRegisteredFile.FileType.Should().Be("text/csv");
            model.ComplianceSchemeRegisteredFile.DatePublished.Should().Be(expectedDate);
            model.ComplianceSchemeRegisteredFile.DateLastModified.Should().Be(expectedLastUpdated);
            model.DefraUrl.Should().NotBeNullOrWhiteSpace();
            _blobStorageServiceMock.Verify(r => r.GetLatestFilePropertiesAsync(containerName), Times.AtMostOnce());
        }

        [TestMethod]
        public async Task Guidance_SetsLastUpdatedToPublishedDate_WhenLastModifiedIsNull()
        {
            // Arrange
            var producerBlob = new PublicRegisterBlobModel
            {
                Name = "producers.csv",
                PublishedDate = _publishedDate,
                LastModified = null,
                ContentLength = "115",
                FileType = "text/csv"
            };

            _blobStorageServiceMock
                .Setup(x => x.GetLatestFilePropertiesAsync("producers-container"))
                .ReturnsAsync(producerBlob);

            // Act
            var result = await _controller.Get();
            var model = (result as ViewResult)!.Model as GuidanceViewModel;

            // Assert
            var expectedDate = _publishedDate.ToString("d MMMM yyyy", CultureInfo.InvariantCulture);
            model!.LastUpdated.Should().Be(expectedDate); // fallback to published date
            model.DefraUrl.Should().NotBeNullOrWhiteSpace();
            _blobStorageServiceMock.Verify(r => r.GetLatestFilePropertiesAsync(It.IsAny<string>()), Times.AtMost(2));
        }

        [TestMethod]
        public async Task Guidance_SetsFileSizeToZero_WhenContentLengthIsNull()
        {
            // Arrange
            var producerBlob = new PublicRegisterBlobModel
            {
                Name = "producers.csv",
                PublishedDate = _publishedDate,
                LastModified = _lastModified,
                ContentLength = null,
                FileType = "text/csv"
            };

            _blobStorageServiceMock
                .Setup(x => x.GetLatestFilePropertiesAsync("producers-container"))
                .ReturnsAsync(producerBlob);

            // Act
            var result = await _controller.Get();
            var model = (result as ViewResult)!.Model as GuidanceViewModel;

            // Assert
            model!.ProducerRegisteredFile.FileSize.Should().Be("0");
            model.DefraUrl.Should().NotBeNullOrWhiteSpace();
            _blobStorageServiceMock.Verify(r => r.GetLatestFilePropertiesAsync(It.IsAny<string>()), Times.AtMost(2));
        }

        [TestMethod]
        public void FormatDate_ReturnsNull_WhenDateIsNull()
        {
            // Act
            var result = typeof(PublicRegisterController)
                .GetMethod("FormatDate", BindingFlags.NonPublic | BindingFlags.Static)!
                .Invoke(null,[null]);

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetFile_Public_Register_ReturnFile()
        {
            // Arrange
            const string filename = "testFileName";
            var fileType = "producers-container";
            var test_Stream = new MemoryStream();

            _blobStorageServiceMock
                 .Setup(x => x.GetLatestFileAsync("producers-container"))
                 .ReturnsAsync(test_Stream);
            // Act
            var result = await _controller.File(filename, fileType) as FileStreamResult;

            // Assert
            result.FileDownloadName.Should().Be(filename);
            result.FileStream.Should().BeSameAs(test_Stream);
            result.ContentType.Should().Be("text/csv");
        }

        [TestMethod]
        public async Task GetFile_Public_Register_RedirectTo_FileNotDownloaded_When_FileContent_Null()
        {
            // Arrange
            const string filename = "testFileName";
            var fileType = "public";
            var test_Stream = new MemoryStream();

            // Act
            var result = await _controller.File(filename, fileType) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result.ActionName.Should().Be("FileNotDownloaded");
        }

        [TestMethod]
        public async Task File_RedirectsToFileNotDownloaded_WhenExceptionThrown()
        {
            // Arrange
            var fileName = "report.csv";
            var fileType = "public";

            _blobStorageServiceMock
                .Setup(s => s.GetLatestFileAsync(It.IsAny<string>()))
                .ThrowsAsync(new PublicRegisterServiceException("fail"));

            // Act
            var result = await _controller.File(fileName, fileType) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result.ActionName.Should().Be("FileNotDownloaded");
        }
    }
}