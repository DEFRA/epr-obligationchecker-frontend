namespace FrontendObligationChecker.UnitTests.Controllers;

using System;
using FluentAssertions;
using FrontendObligationChecker.Controllers;
using FrontendObligationChecker.Extensions;
using FrontendObligationChecker.Models.Config;
using FrontendObligationChecker.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using Moq;

[TestClass]
public class AccessibilityControllerTests
{
    private Mock<HttpContext>? _httpContextMock;
    private Mock<HttpRequest> _httpRequest;

    private Mock<IOptions<ExternalUrlsOptions>>? _urlOptions;
    private Mock<IOptions<EmailAddressOptions>>? _emailOptions;
    private Mock<IOptions<SiteDateOptions>>? _siteDateOptions;

    private AccessibilityController? _systemUnderTest;

    [TestInitialize]
    public void TestInitialize()
    {
        _httpContextMock = new Mock<HttpContext>();
        _httpRequest = new Mock<HttpRequest>();
        _urlOptions = new Mock<IOptions<ExternalUrlsOptions>>();
        _emailOptions = new Mock<IOptions<EmailAddressOptions>>();
        _siteDateOptions = new Mock<IOptions<SiteDateOptions>>();

        SetUpConfigOption();

        _systemUnderTest = new AccessibilityController(
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
        var accessibilityViewModel = (AccessibilityViewModel)viewResult.Model;
        var expectedSiteTestedDate = _siteDateOptions.Object.Value.AccessibilitySiteTested.ToString(_siteDateOptions.Object.Value.DateFormat);
        var expectedStatementPreparedDate = _siteDateOptions.Object.Value.AccessibilityStatementPrepared.ToString(_siteDateOptions.Object.Value.DateFormat);
        var expectedStatementReviewedDate = _siteDateOptions.Object.Value.AccessibilityStatementReviewed.ToString(_siteDateOptions.Object.Value.DateFormat);

        // Assert
        result.Should().BeOfType(typeof(ViewResult));
        accessibilityViewModel.CurrentPage.Should().Be(returnUrl);
        accessibilityViewModel.AbilityNetUrl.Should().Be(_urlOptions.Object.Value.AccessibilityAbilityNet);
        accessibilityViewModel.ContactUsUrl.Should().Be(_urlOptions.Object.Value.AccessibilityContactUs);
        accessibilityViewModel.EqualityAdvisorySupportServiceUrl.Should().Be(_urlOptions.Object.Value.AccessibilityEqualityAdvisorySupportService);
        accessibilityViewModel.DefraHelplineEmail.Should().Be(_emailOptions.Object.Value.DefraHelpline);
        accessibilityViewModel.SiteTestedDate.Should().Be(expectedSiteTestedDate);
        accessibilityViewModel.StatementPreparedDate.Should().Be(expectedStatementPreparedDate);
        accessibilityViewModel.StatementReviewedDate.Should().Be(expectedStatementReviewedDate);
        accessibilityViewModel.WebContentAccessibilityUrl.Should().Be(_urlOptions.Object.Value.AccessibilityWebContentAccessibility);
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
        var accessibilityViewModel = (AccessibilityViewModel)viewResult.Model;

        // Assert
        result.Should().BeOfType(typeof(ViewResult));
        accessibilityViewModel.CurrentPage.Should().Be(homeUrl);
    }

    private void SetUpConfigOption()
    {
        var externalUrlsOptions = new ExternalUrlsOptions()
        {
            AccessibilityAbilityNet = "url1",
            AccessibilityContactUs = "url2",
            AccessibilityEqualityAdvisorySupportService = "url3",
            PrivacyGoogleAnalytics = "url4",
        };

        var emailAddressOptions = new EmailAddressOptions()
        {
            DefraHelpline = "1@email.com"
        };

        var siteDateOptions = new SiteDateOptions()
        {
            AccessibilitySiteTested = DateTime.Parse("2000-01-01"),
            AccessibilityStatementPrepared = DateTime.Parse("2001-01-01"),
            AccessibilityStatementReviewed = DateTime.Parse("2002-01-01"),
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