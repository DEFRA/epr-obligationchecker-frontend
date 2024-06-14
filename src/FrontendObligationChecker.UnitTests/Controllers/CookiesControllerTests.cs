namespace FrontendObligationChecker.UnitTests.Controllers;

using FluentAssertions;
using FrontendObligationChecker.Controllers;
using FrontendObligationChecker.Extensions;
using FrontendObligationChecker.Models.Config;
using FrontendObligationChecker.Models.Cookies;
using FrontendObligationChecker.Services.Infrastructure.Interfaces;
using FrontendObligationChecker.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using Moq;

[TestClass]
public class CookiesControllerTests
{
    private const string CookieName = ".epr_cookie_policy";
    private const string GoogleAnalyticsDefaultCookieName = "_ga";

    private Mock<HttpContext>? _httpContextMock;
    private Mock<HttpRequest> _httpRequest;
    private CookieConsentState _cookieConsentState;

    private Mock<IOptions<EprCookieOptions>> _cookieOptions;
    private Mock<IOptions<AnalyticsOptions>> _googleAnalyticsOptions;
    private Mock<ICookieService> _cookieService;

    private CookiesController? _systemUnderTest;

    [TestInitialize]
    public void TestInitialize()
    {
        _httpContextMock = new Mock<HttpContext>();
        _httpRequest = new Mock<HttpRequest>();

        var requestCookiesMock = new Mock<IRequestCookieCollection>();
        var responseCookiesMock = new Mock<IResponseCookies>();
        _httpContextMock.Setup(ctx => ctx.Request.Cookies).Returns(requestCookiesMock.Object);
        _httpContextMock.Setup(ctx => ctx.Response.Cookies).Returns(responseCookiesMock.Object);

        _cookieConsentState = new CookieConsentState
        {
            CookieAcknowledgementRequired = true,
            CookiesAccepted = true,
            CookieExists = true
        };

        _cookieService = new Mock<ICookieService>();
        _cookieService
            .Setup(ap => ap.GetConsentState(It.IsAny<IRequestCookieCollection>(), It.IsAny<IResponseCookies>()))
        .Returns(_cookieConsentState);

        SetUpConfigOption();

        _systemUnderTest = new CookiesController(
            _cookieService.Object,
            _cookieOptions.Object,
            _googleAnalyticsOptions.Object);

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
        var cookieDetailViewModel = (CookieDetailViewModel)viewResult.Model;

        // Assert
        result.Should().BeOfType(typeof(ViewResult));
        cookieDetailViewModel.CurrentPage.Should().Be(returnUrl);
        cookieDetailViewModel.SessionCookieName.Should().Be(_cookieOptions.Object.Value.SessionCookieName);
        cookieDetailViewModel.CookiePolicyCookieName.Should().Be(_cookieOptions.Object.Value.CookiePolicyCookieName);
        cookieDetailViewModel.AntiForgeryCookieName.Should().Be(_cookieOptions.Object.Value.AntiForgeryCookieName);
        cookieDetailViewModel.GoogleAnalyticsDefaultCookieName.Should().Be(_googleAnalyticsOptions.Object.Value.DefaultCookieName);
        cookieDetailViewModel.GoogleAnalyticsAdditionalCookieName.Should().Be(_googleAnalyticsOptions.Object.Value.AdditionalCookieName);
        cookieDetailViewModel.CookiesAccepted.Should().Be(_cookieConsentState.CookiesAccepted);
        cookieDetailViewModel.ShowAcknowledgement.Should().Be(_cookieConsentState.CookieAcknowledgementRequired);
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
        var cookieDetailViewModel = (CookieDetailViewModel)viewResult.Model;

        // Assert
        result.Should().BeOfType(typeof(ViewResult));
        cookieDetailViewModel.CurrentPage.Should().Be(homeUrl);
    }

    [TestMethod]
    public async Task ConfirmAcceptance_Returns_LocalRedirect()
    {
        // Arrange
        const string returnUrl = "/obligation-checker/question";
        var expectedUrl = returnUrl;

        // Act
        var result = _systemUnderTest!.ConfirmAcceptance(returnUrl);

        // Assert
        result.Should().NotBeNull();
        result.Url.Should().Be(expectedUrl);
    }

    [TestMethod]
    public async Task UpdateAcceptance_Returns_LocalRedirect()
    {
        // Arrange
        const string returnUrl = "/obligation-checker/question";
        var cookies = CookieAcceptance.Accept;
        var expectedUrl = returnUrl;

        // Act
        var result = _systemUnderTest!.UpdateAcceptance(returnUrl, cookies);

        // Assert
        result.Should().NotBeNull();
        result.Url.Should().Be(expectedUrl);

        _cookieService.Verify(x => x.SetCookieAcceptance(true, It.IsAny<IRequestCookieCollection>(), It.IsAny<IResponseCookies>()), Times.Once);
    }

    private void SetUpConfigOption()
    {
        var eprCookieOptions = new EprCookieOptions() { CookiePolicyCookieName = CookieName };
        var googleAnalyticsOptions = new AnalyticsOptions { CookiePrefix = GoogleAnalyticsDefaultCookieName };

        _cookieOptions = new Mock<IOptions<EprCookieOptions>>();
        _cookieOptions
            .Setup(ap => ap.Value)
            .Returns(eprCookieOptions);

        _googleAnalyticsOptions = new Mock<IOptions<AnalyticsOptions>>();
        _googleAnalyticsOptions
            .Setup(ap => ap.Value)
            .Returns(googleAnalyticsOptions);
    }
}