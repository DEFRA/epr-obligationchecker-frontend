namespace FrontendObligationChecker.UnitTests.Services.Infrastructure;

using FluentAssertions;
using FrontendObligationChecker.Models.Config;
using FrontendObligationChecker.Services.Infrastructure;
using FrontendObligationChecker.Services.Wrappers.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Moq;

[TestClass]
public class CookieServiceTests
{
    private const string CookieName = ".epr_cookie_policy";
    private const string GoogleAnalyticsDefaultCookieName = "_ga";

    private CookieService _systemUnderTest;
    private Mock<IOptions<EprCookieOptions>> _cookieOptions;
    private Mock<IOptions<AnalyticsOptions>> _googleAnalyticsOptions;
    private Mock<ILogger<CookieService>> _loggerMock;
    private Mock<IDateTimeWrapper> _dateTimeWrapperMock;
    private Mock<EprCookieOptions> _eprCookieOptions;

    [TestInitialize]
    public void TestInitialize()
    {
        _cookieOptions = new Mock<IOptions<EprCookieOptions>>();
        _loggerMock = new Mock<ILogger<CookieService>>();
        _dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
        _eprCookieOptions = new Mock<EprCookieOptions>();

        SetDateTimeWrapperMock();
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public async Task SetCookieAcceptance_LogsError_WhenArgumentNullExceptionThrow()
    {
        // Arrange
        const string expectedLog = "Error setting cookie acceptance to 'True'";
        var requestCookieCollection = MockRequestCookieCollection("test", "test");
        HttpContext context = new DefaultHttpContext();
        MockService();

        // Act
        _systemUnderTest.SetCookieAcceptance(true, requestCookieCollection, context.Response.Cookies);

        // Assert
        _loggerMock.VerifyLog(logger => logger.LogError(expectedLog), Times.Once);
    }

    [TestMethod]
    public async Task SetCookieAcceptance_True_ReturnValidCookie()
    {
        // Arrange
        var requestCookieCollection = MockRequestCookieCollection();
        var context = new DefaultHttpContext();
        MockService(CookieName);

        // Act
        _systemUnderTest.SetCookieAcceptance(true, requestCookieCollection, context.Response.Cookies);

        // Assert
        var cookieValue = GetCookieValueFromResponse(context.Response, CookieName);
        cookieValue.Should().Contain("True");
    }

    [TestMethod]
    public async Task SetCookieAcceptance_False_ReturnValidCookie()
    {
        // Arrange
        var requestCookieCollection = MockRequestCookieCollection();
        var context = new DefaultHttpContext();
        MockService(CookieName);

        // Act
        _systemUnderTest.SetCookieAcceptance(false, requestCookieCollection, context.Response.Cookies);

        // Assert
        var cookieValue = GetCookieValueFromResponse(context.Response, CookieName);
        cookieValue.Should().Contain("False");
    }

    [TestMethod]
    public async Task SetCookieAcceptance_False_ResetsGACookie()
    {
        // Arrange
        var requestCookieCollection = MockRequestCookieCollection(GoogleAnalyticsDefaultCookieName, "1234");
        var context = new DefaultHttpContext();
        MockService(CookieName);

        // Act
        _systemUnderTest.SetCookieAcceptance(false, requestCookieCollection, context.Response.Cookies);

        // Assert
        var cookieValue = GetCookieValueFromResponse(context.Response, GoogleAnalyticsDefaultCookieName);
        cookieValue.Should().Be("1234");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public async Task GetConsentState_LogsError_WhenArgumentNullExceptionThrow()
    {
        // Arrange
        const string expectedLog = "Error reading cookie acceptance";
        var requestCookieCollection = MockRequestCookieCollection("test", "test");
        var context = new DefaultHttpContext();
        MockService();

        // Act
        _systemUnderTest.GetConsentState(requestCookieCollection, context.Response.Cookies);

        // Assert
        _loggerMock.VerifyLog(logger => logger.LogError(expectedLog), Times.Once);

    }

    [TestMethod]
    public async Task GetConsentState_True_ReturnsValidValue()
    {
        // Arrange
        var requestCookieCollection = MockRequestCookieCollection(CookieName, "True");
        var context = new DefaultHttpContext();
        MockService(CookieName);

        // Act
        var result = _systemUnderTest.GetConsentState(requestCookieCollection, context.Response.Cookies);

        // Assert
        result.CookieExists.Should().BeTrue();
        result.CookiesAccepted.Should().BeTrue();
        result.CookieAcknowledgementRequired.Should().BeFalse();
    }

    [TestMethod]
    public async Task GetConsentState_TrueWithAck_ReturnsValidValue()
    {
        // Arrange
        var requestCookieCollection = MockRequestCookieCollection(CookieName, "True|ACK");
        var context = new DefaultHttpContext();
        MockService(CookieName);

        // Act
        var result = _systemUnderTest.GetConsentState(requestCookieCollection, context.Response.Cookies);

        // Assert
        result.CookieExists.Should().BeTrue();
        result.CookiesAccepted.Should().BeTrue();
        result.CookieAcknowledgementRequired.Should().BeTrue();
    }

    [TestMethod]
    public async Task GetConsentState_False_ReturnsValidValue()
    {
        // Arrange
        var requestCookieCollection = MockRequestCookieCollection(CookieName, "False");
        var context = new DefaultHttpContext();
        MockService(CookieName);

        // Act
        var result = _systemUnderTest.GetConsentState(requestCookieCollection, context.Response.Cookies);

        // Assert
        result.CookieExists.Should().BeTrue();
        result.CookiesAccepted.Should().BeFalse();
        result.CookieAcknowledgementRequired.Should().BeFalse();
    }

    [TestMethod]
    public async Task GetConsentState_FalseWithAck_ReturnsValidValue()
    {
        // Arrange
        var requestCookieCollection = MockRequestCookieCollection(CookieName, "False|ACK");
        var context = new DefaultHttpContext();
        MockService(CookieName);

        // Act
        var result = _systemUnderTest.GetConsentState(requestCookieCollection, context.Response.Cookies);

        // Assert
        result.CookieExists.Should().BeTrue();
        result.CookiesAccepted.Should().BeFalse();
        result.CookieAcknowledgementRequired.Should().BeTrue();
    }

    [TestMethod]
    public async Task GetConsentState_NoCookie_ReturnsValidValue()
    {
        // Arrange
        var requestCookieCollection = MockRequestCookieCollection("test", "test");
        var context = new DefaultHttpContext();
        MockService(CookieName);

        // Act
        var result = _systemUnderTest.GetConsentState(requestCookieCollection, context.Response.Cookies);

        // Assert
        result.CookieExists.Should().BeFalse();
        result.CookiesAccepted.Should().BeFalse();
        result.CookieAcknowledgementRequired.Should().BeFalse();
    }

    private static IRequestCookieCollection MockRequestCookieCollection(string key = "", string value = "")
    {
        var requestFeature = new HttpRequestFeature();
        var featureCollection = new FeatureCollection();
        requestFeature.Headers = new HeaderDictionary();
        if (key != string.Empty && value != string.Empty)
        {
            requestFeature.Headers.Add(HeaderNames.Cookie, new StringValues(key + "=" + value));
        }

        featureCollection.Set<IHttpRequestFeature>(requestFeature);
        var cookiesFeature = new RequestCookiesFeature(featureCollection);
        return cookiesFeature.Cookies;
    }

    private string GetCookieValueFromResponse(HttpResponse response, string cookieName)
    {
        foreach (var headers in response.Headers)
        {
            if (headers.Key != "Set-Cookie")
                continue;
            string header = headers.Value;
            if (!header.StartsWith($"{cookieName}="))
            {
                continue;
            }

            var p1 = header.IndexOf('=');
            var p2 = header.IndexOf(';');
            return header.Substring(p1 + 1, p2 - p1 - 1);
        }

        return null;
    }

    private void SetDateTimeWrapperMock()
    {
        _dateTimeWrapperMock.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);
    }

    private void MockService(string cookieName = null)
    {
        var eprCookieOptions = new EprCookieOptions() { CookiePolicyCookieName = cookieName };
        var googleAnalyticsOptions = new AnalyticsOptions { CookiePrefix = GoogleAnalyticsDefaultCookieName };

        _cookieOptions = new Mock<IOptions<EprCookieOptions>>();
        _cookieOptions.Setup(ap => ap.Value).Returns(eprCookieOptions);

        _googleAnalyticsOptions = new Mock<IOptions<AnalyticsOptions>>();
        _googleAnalyticsOptions.Setup(ap => ap.Value).Returns(googleAnalyticsOptions);

        _systemUnderTest = new CookieService(
            _loggerMock.Object,
            _dateTimeWrapperMock.Object,
            _cookieOptions.Object,
            _googleAnalyticsOptions.Object);
    }
}