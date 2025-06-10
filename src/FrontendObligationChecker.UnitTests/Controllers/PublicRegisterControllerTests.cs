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
    using FrontendObligationChecker.Services.PublicRegister.Interfaces;
    using FrontendObligationChecker.ViewModels.PublicRegister;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class PublicRegisterControllerTests
    {
        private Mock<IBlobStorageService> _blobStorageServiceMock = null!;
        private Mock<IFeatureFlagService> _mockFeatureFlagService = null!;
        private IOptions<PublicRegisterOptions> _publicRegisterOptions = null!;
        private IOptions<ExternalUrlsOptions> _externalUrlsOptions = null!;
        private IOptions<EmailAddressOptions> _emailAddressOptions = null!;
        private PublicRegisterController _controller = null!;
        private DateTime _publishedDate;
        private DateTime _lastModified;

        [TestInitialize]
        public void Setup()
        {
            _blobStorageServiceMock = new Mock<IBlobStorageService>();
            _mockFeatureFlagService = new Mock<IFeatureFlagService>();

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
                DefraUrl = "https://www.defraurl.com",
                PublicRegisterScottishProtectionAgency = "some/url.com"
            });

            _emailAddressOptions = Options.Create(new EmailAddressOptions
            {
                DefraHelpline = "defrahelpline@email.com"
            });

            _controller = new PublicRegisterController(
                _blobStorageServiceMock.Object,
                _externalUrlsOptions,
                _emailAddressOptions,
                _publicRegisterOptions,
                _mockFeatureFlagService.Object);
        }

        [TestMethod]
        public async Task Get_ReturnsExpectedViewWithCorrectModel_WhenComplianceSchemesRegisterDisabled_AndEnforcementActionsSectionEnabled()
        {
            // Arrange
            _mockFeatureFlagService.Setup(mock =>
                mock.IsComplianceSchemesRegisterEnabledAsync()).ReturnsAsync(false);

            _mockFeatureFlagService.Setup(mock =>
                mock.IsEnforcementActionsSectionEnabledAsync()).ReturnsAsync(true);

            var expectedEnforcementActionFiles = new List<EnforcementActionFileViewModel>
            {
                new ()
                {
                    FileName = "Document1_EA.xlsx",
                    DateCreated = DateTime.Now.AddDays(-10),
                    ContentFileLength = 1024,
                    FileContents = new MemoryStream(new byte[1024])
                },
                new ()
                {
                    FileName = "Document2_NRW.xlsx",
                    DateCreated = DateTime.Now.AddDays(-5),
                    ContentFileLength = 2048,
                    FileContents = new MemoryStream(new byte[2048])
                },
                new ()
                {
                    FileName = "Document3_NIEA.xlsx",
                    DateCreated = DateTime.Now,
                    ContentFileLength = 512,
                    FileContents = new MemoryStream(new byte[512])
                }
            };

            _blobStorageServiceMock.Setup(bsm =>
                bsm.GetEnforcementActionFiles()).ReturnsAsync(expectedEnforcementActionFiles);

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
            model.DefraHelplineEmail.Should().NotBeNullOrEmpty();

            model.ProducerRegisteredFile.Should().NotBeNull();
            model.ProducerRegisteredFile!.FileName.Should().Be("producers.csv");
            model.ProducerRegisteredFile.FileSize.Should().Be("115");
            model.ProducerRegisteredFile.FileType.Should().Be("text/csv");
            model.ProducerRegisteredFile.DatePublished.Should().Be(expectedDate);
            model.ProducerRegisteredFile.DateLastModified.Should().Be(expectedLastUpdated);

            model.ComplianceSchemeRegisteredFile.Should().BeNull();

            model.EnforcementActionFiles.Should().NotBeNullOrEmpty();
            model.EnforcementActionFiles!.Should().HaveCount(3);

            model.EnglishEnforcementActionFile.Should().NotBeNull();
            model.EnglishEnforcementActionFile.FileDownloadUrl.Should().NotBeNull();

            model.WelshEnforcementActionFile.Should().NotBeNull();
            model.WelshEnforcementActionFile.FileDownloadUrl.Should().NotBeNull();

            model.NortherIrishEnforcementActionFile.Should().NotBeNull();
            model.NortherIrishEnforcementActionFile.FileDownloadUrl.Should().NotBeNull();

            model.ScottishEnforcementActionFileUrl.Should().NotBeNull();

            model.DefraUrl.Should().NotBeNullOrWhiteSpace();

            _blobStorageServiceMock.Verify(r => r.GetLatestFilePropertiesAsync("producers-container"), Times.AtMostOnce());
            _blobStorageServiceMock.Verify(bsm => bsm.GetEnforcementActionFiles(), Times.AtMostOnce());
        }

        [TestMethod]
        public async Task Get_ReturnsExpectedViewWithCorrectModel_WhenComplianceSchemesRegisterEnabled_AndEnforcementActionsSectionDisabled()
        {
            // Arrange
            _mockFeatureFlagService.Setup(mock =>
                mock.IsComplianceSchemesRegisterEnabledAsync()).ReturnsAsync(true);

            _mockFeatureFlagService.Setup(mock =>
                mock.IsEnforcementActionsSectionEnabledAsync()).ReturnsAsync(false);

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
            model.DefraHelplineEmail.Should().NotBeNullOrEmpty();

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

            model.EnforcementActionFiles.Should().BeNull();

            model.DefraUrl.Should().NotBeNullOrWhiteSpace();

            _blobStorageServiceMock.Verify(r => r.GetLatestFilePropertiesAsync("producers-container"), Times.AtMostOnce());
            _blobStorageServiceMock.Verify(bsm => bsm.GetEnforcementActionFiles(), Times.Never());
        }

        [TestMethod]
        public async Task Get_ReturnsExpectedViewWithCorrectModel_WhenComplianceSchemesRegisterEnabled_AndEnforcementActionsSectionEnabled()
        {
            // Arrange
            _mockFeatureFlagService.Setup(mock =>
                mock.IsComplianceSchemesRegisterEnabledAsync()).ReturnsAsync(true);

            _mockFeatureFlagService.Setup(mock =>
                mock.IsEnforcementActionsSectionEnabledAsync()).ReturnsAsync(true);

            var expectedEnforcementActionFiles = new List<EnforcementActionFileViewModel>
            {
                new ()
                {
                    FileName = "Document1_EA.xlsx",
                    DateCreated = DateTime.Now.AddDays(-10),
                    ContentFileLength = 1024,
                    FileContents = new MemoryStream(new byte[1024])
                },
                new ()
                {
                    FileName = "Document2_NRW.xlsx",
                    DateCreated = DateTime.Now.AddDays(-5),
                    ContentFileLength = 2048,
                    FileContents = new MemoryStream(new byte[2048])
                },
                new ()
                {
                    FileName = "Document3_NIEA.xlsx",
                    DateCreated = DateTime.Now,
                    ContentFileLength = 512,
                    FileContents = new MemoryStream(new byte[512])
                }
            };

            _blobStorageServiceMock.Setup(bsm =>
                bsm.GetEnforcementActionFiles()).ReturnsAsync(expectedEnforcementActionFiles);

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
            model.DefraHelplineEmail.Should().NotBeNullOrEmpty();

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

            model.EnforcementActionFiles.Should().NotBeNullOrEmpty();
            model.EnforcementActionFiles!.Should().HaveCount(3);

            model.EnglishEnforcementActionFile.Should().NotBeNull();
            model.EnglishEnforcementActionFile.FileDownloadUrl.Should().NotBeNull();

            model.WelshEnforcementActionFile.Should().NotBeNull();
            model.WelshEnforcementActionFile.FileDownloadUrl.Should().NotBeNull();

            model.NortherIrishEnforcementActionFile.Should().NotBeNull();
            model.NortherIrishEnforcementActionFile.FileDownloadUrl.Should().NotBeNull();

            model.ScottishEnforcementActionFileUrl.Should().NotBeNull();

            model.DefraUrl.Should().NotBeNullOrWhiteSpace();

            _blobStorageServiceMock.Verify(r => r.GetLatestFilePropertiesAsync("producers-container"), Times.AtMostOnce());
            _blobStorageServiceMock.Verify(bsm => bsm.GetEnforcementActionFiles(), Times.AtMostOnce());
        }

        [TestMethod]
        public async Task Get_ReturnsExpectedViewWithCorrectModel_WhenComplianceSchemesRegisterDisabled_AndEnforcementActionsSectionDisabled()
        {
            // Arrange
            _mockFeatureFlagService.Setup(mock =>
                mock.IsComplianceSchemesRegisterEnabledAsync()).ReturnsAsync(false);

            _mockFeatureFlagService.Setup(mock =>
                mock.IsEnforcementActionsSectionEnabledAsync()).ReturnsAsync(false);

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
            model.DefraHelplineEmail.Should().NotBeNullOrEmpty();

            model.PublishedDate.Should().Be(expectedDate);
            model.LastUpdated.Should().Be(expectedLastUpdated);

            model.ProducerRegisteredFile.Should().NotBeNull();
            model.ProducerRegisteredFile!.FileName.Should().Be("producers.csv");
            model.ProducerRegisteredFile.FileSize.Should().Be("115");
            model.ProducerRegisteredFile.FileType.Should().Be("text/csv");
            model.ProducerRegisteredFile.DatePublished.Should().Be(expectedDate);
            model.ProducerRegisteredFile.DateLastModified.Should().Be(expectedLastUpdated);

            model.ComplianceSchemeRegisteredFile.Should().BeNull();

            model.EnforcementActionFiles.Should().BeNull();

            model.DefraUrl.Should().NotBeNullOrWhiteSpace();

            _blobStorageServiceMock.Verify(r => r.GetLatestFilePropertiesAsync("producers-container"), Times.AtMostOnce());
            _blobStorageServiceMock.Verify(bsm => bsm.GetEnforcementActionFiles(), Times.Never());
        }

        [TestMethod]
        public async Task Get_SetsLastUpdatedToPublishedDate_WhenLastModifiedIsNull()
        {
            // Arrange
            var producerBlob = new PublicRegisterBlobModel
            {
                Name = "producers.csv",
                PublishedDate = _publishedDate,
                LastModified = null,
                ContentLength = "115",
                FileType = "text/csv",
                EnforcementActionItems = new List<Azure.Storage.Blobs.Models.BlobItem>()
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
        public async Task Get_SetsFileSizeToZero_WhenContentLengthIsNull()
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
            var faileModel = new PublicRegisterFileModel { FileName = filename, FileContent = new MemoryStream() };

            _blobStorageServiceMock
                 .Setup(x => x.GetLatestFileAsync("producers-container"))
                 .ReturnsAsync(faileModel);
            // Act
            var result = await _controller.File(filename, fileType) as FileStreamResult;

            // Assert
            result.FileDownloadName.Should().Be(faileModel.FileName);
            result.FileStream.Should().BeSameAs(faileModel.FileContent);
            result.ContentType.Should().Be("text/csv");
        }

        [TestMethod]
        public async Task GetFile_Public_Register_RedirectTo_FileNotDownloaded_When_FileContent_Null()
        {
            // Arrange
            const string filename = "testFileName";
            var fileType = "producers-container";
            var faileModel = new PublicRegisterFileModel();

            _blobStorageServiceMock
                .Setup(x => x.GetLatestFileAsync("producers-container"))
                .ReturnsAsync(faileModel);

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

        [TestMethod]
        public async Task GetFileNotDownloaded_ReturnsLargeProducerErrorView_WhenCalledWithValidHomeNation()
        {
            // Act
            var result = await _controller.FileNotDownloaded() as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().Be("GuidanceError");
        }
    }
}