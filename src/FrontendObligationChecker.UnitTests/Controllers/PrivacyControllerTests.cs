namespace FrontendObligationChecker.UnitTests.Controllers;

using FluentAssertions;
using FrontendObligationChecker.Controllers;
using FrontendObligationChecker.Extensions;
using FrontendObligationChecker.Models.Config;
using FrontendObligationChecker.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

[TestClass]
public class PrivacyControllerTests
{
    private Mock<ILogger<PrivacyController>>? _loggerMock;
    private Mock<HttpContext>? _httpContextMock;
    private Mock<HttpRequest> _httpRequest;

    private Mock<IOptions<ExternalUrlsOptions>>? _urlOptions;
    private Mock<IOptions<EmailAddressOptions>>? _emailOptions;
    private Mock<IOptions<SiteDateOptions>>? _siteDateOptions;

    private PrivacyController? _systemUnderTest;

    [TestInitialize]
    public void TestInitialize()
    {
        _loggerMock = new Mock<ILogger<PrivacyController>>();
        _httpContextMock = new Mock<HttpContext>();
        _httpRequest = new Mock<HttpRequest>();
        _urlOptions = new Mock<IOptions<ExternalUrlsOptions>>();
        _emailOptions = new Mock<IOptions<EmailAddressOptions>>();
        _siteDateOptions = new Mock<IOptions<SiteDateOptions>>();

        SetUpConfigOption();

        _systemUnderTest = new PrivacyController(
            _urlOptions.Object,
            _emailOptions.Object,
            _siteDateOptions.Object);

        _systemUnderTest.ControllerContext.HttpContext = _httpContextMock.Object;
        _httpContextMock.Setup(x => x.Request).Returns(_httpRequest.Object);
    }

    [TestMethod]
    public async Task Detail_SetsModel()
    {
        // Arrange
        const string returnUrl = "/obligation-checker/question";
        var mockUrlHelper = new Mock<IUrlHelper>();
        mockUrlHelper
            .Setup(m => m.IsLocalUrl(It.IsAny<string>()))
            .Returns(true)
            .Verifiable();
        _systemUnderTest.Url = mockUrlHelper.Object;

        // Act
        var result = await _systemUnderTest!.Detail(returnUrl);
        var viewResult = result as ViewResult;
        var privacyViewModel = (PrivacyViewModel)viewResult.Model;
        var expectedDate = _siteDateOptions.Object.Value.PrivacyLastUpdated.ToString(_siteDateOptions.Object.Value.DateFormat);

        // Assert
        result.Should().BeOfType(typeof(ViewResult));
        privacyViewModel.LastUpdated.Should().Be(expectedDate);
        privacyViewModel.DataProtectionEmail.Should().Be(_emailOptions.Object.Value.DataProtection);
        privacyViewModel.InformationCommissionerEmail.Should().Be(_emailOptions.Object.Value.InformationCommissioner);
        privacyViewModel.DefraGroupProtectionOfficerEmail.Should().Be(_emailOptions.Object.Value.DefraGroupProtectionOfficer);
        privacyViewModel.DataProtectionPublicRegisterUrl.Should().Be(_urlOptions.Object.Value.PrivacyDataProtectionPublicRegister);
        privacyViewModel.WebBrowserUrl.Should().Be(_urlOptions.Object.Value.PrivacyWebBrowser);
        privacyViewModel.GoogleAnalyticsUrl.Should().Be(_urlOptions.Object.Value.PrivacyGoogleAnalytics);
        privacyViewModel.DefrasPersonalInformationCharterUrl.Should().Be(_urlOptions.Object.Value.PrivacyDefrasPersonalInformationCharter);
        privacyViewModel.InformationCommissionerUrl.Should().Be(_urlOptions.Object.Value.PrivacyInformationCommissioner);
        privacyViewModel.FindOutAboutCallChargesUrl.Should().Be(_urlOptions.Object.Value.PrivacyFindOutAboutCallCharges);
    }

    [TestMethod]
    public async Task Detail_Sets_HomeUrl_IfNotLocalUrl()
    {
        // Arrange
        const string homeUrl = "/obligation-checker/home";
        const string returnUrl = "https://external.com";
        var mockUrlHelper = new Mock<IUrlHelper>();
        mockUrlHelper
            .Setup(m => m.IsLocalUrl(It.IsAny<string>()))
            .Returns(false)
            .Verifiable();
        mockUrlHelper
            .Setup(m => m.Action(It.IsAny<UrlActionContext>()))
            .Returns(homeUrl)
            .Verifiable();
        _systemUnderTest.Url = mockUrlHelper.Object;

        var expectedUrl = _systemUnderTest.Url.HomePath();

        // Act
        var result = await _systemUnderTest!.Detail(returnUrl);
        var viewResult = result as ViewResult;
        var privacyViewModel = (PrivacyViewModel)viewResult.Model;

        // Assert
        result.Should().BeOfType(typeof(ViewResult));
        privacyViewModel.CurrentPage.Should().Be(homeUrl);
    }

    private void SetUpConfigOption()
    {
        var externalUrlsOptions = new ExternalUrlsOptions()
        {
            EprGuidance = "url1",
            PrivacyDataProtectionPublicRegister = "url2",
            PrivacyWebBrowser = "url3",
            PrivacyGoogleAnalytics = "url4",
            PrivacyEuropeanEconomicArea = "url5",
            PrivacyDefrasPersonalInformationCharter = "url6",
            PrivacyInformationCommissioner = "url7",
            PrivacyFindOutAboutCallCharges = "url8"
        };

        var emailAddressOptions = new EmailAddressOptions()
        {
            DataProtection = "1@email.com",
            DefraGroupProtectionOfficer = "2@email.com",
            InformationCommissioner = "3@email.com"
        };

        var siteDateOptions = new SiteDateOptions()
        {
            PrivacyLastUpdated = DateTime.Parse("2000-01-01"),
            DateFormat = "d MMMM yyyy"
        };

        _urlOptions!
            .Setup(x => x.Value)
            .Returns(externalUrlsOptions);

        _emailOptions!
            .Setup(x => x.Value)
            .Returns(emailAddressOptions);

        _siteDateOptions!
            .Setup(x => x.Value)
            .Returns(siteDateOptions);
    }
}