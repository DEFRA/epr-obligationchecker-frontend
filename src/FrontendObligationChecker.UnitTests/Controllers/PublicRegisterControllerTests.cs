namespace FrontendObligationChecker.UnitTests.Controllers
{
    using System.Threading.Tasks;
    using Azure.Storage.Blobs.Models;
    using FrontendObligationChecker.Controllers;
    using FrontendObligationChecker.Models.BlobReader;
    using FrontendObligationChecker.Services.PublicRegister;
    using FrontendObligationChecker.Sessions;
    using FrontendObligationChecker.ViewModels.PublicRegister;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class PublicRegisterControllerTests
    {
        private Mock<SessionRequestCultureProvider> _mockCultureProvider;
        private Mock<IBlobStorageService> _blobStorageService;
        private Mock<HttpContext>? _mockHttpContext;
        private Mock<ISession> _mockSession;
        private PublicRegisterController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockCultureProvider = new Mock<SessionRequestCultureProvider>();
            _mockSession = new Mock<ISession>();
            _mockHttpContext = new Mock<HttpContext>();
            _mockHttpContext.Setup(mock => mock.Session).Returns(_mockSession.Object);
            _blobStorageService = new Mock<IBlobStorageService>();

            var blopProp = new BlobProperties();

            _blobStorageService.Setup(x => x.GetLatestProducersFilePropertiesAsync()).ReturnsAsync(new PublicRegisterBlobModel
            {
                PublishedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                ContentLength = "100",
                Name = "Public_Register_Producers_10_April_2025.csv"
            });
            _controller = new PublicRegisterController(_blobStorageService.Object);
        }

        [TestMethod]
        public async Task PublicRegisterGuidance_ShouldReturnView_WithCorrectViewModel()
        {
            // Arrange
            var expectedViewModel = new GuidanceViewModel
            {
                ComplianceSchemesRegisteredFileSize = "450",
                LastUpdated = "10 March 2025",
                ProducersRegisteredFileSize = "115",
                PublishedDate = "6 December 2025"
            };

            // Act
            var result = await _controller.Guidance() as ViewResult;

            // Assert
            Assert.IsNotNull(result, "Result should not be null.");
            Assert.AreEqual("Guidance", result.ViewName, "The view name should be 'Guidance'.");

            Assert.IsNotNull(result.Model);
            Assert.IsInstanceOfType(expectedViewModel, result.Model.GetType());
        }
    }
}