namespace FrontendObligationChecker.UnitTests.Controllers;

using System.Net;
using FluentAssertions;
using FrontendObligationChecker.Controllers;
using FrontendObligationChecker.Models.ObligationChecker;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

[TestClass]
public class ErrorControllerTests
{
    private ErrorController _systemUnderTest;
    private Mock<HttpResponse> _mockHttpResponse;

    [TestInitialize]
    public void Init()
    {
        var mockHttpContext = new Mock<HttpContext>();
        _mockHttpResponse = new Mock<HttpResponse>();

        _mockHttpResponse.SetupProperty(r => r.StatusCode, (int)HttpStatusCode.OK);
        mockHttpContext.Setup(c => c.Response).Returns(_mockHttpResponse.Object);

        var controllerContext = new ControllerContext()
        {
            HttpContext = mockHttpContext.Object
        };

        _systemUnderTest = new ErrorController
        {
            ControllerContext = controllerContext
        };
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow((int)HttpStatusCode.NotFound)]
    [DataRow((int)HttpStatusCode.InternalServerError)]
    [DataRow((int)HttpStatusCode.BadRequest)]
    public void Error_ReturnsErrorView_WhenCalledWithErrorCode(int? statusCode)
    {
        // Arrange
        var httpStatusCode = (HttpStatusCode?)statusCode;
        var expectedPageName = httpStatusCode switch
        {
            HttpStatusCode.NotFound => nameof(PageType.PageNotFound),
            _ => nameof(PageType.Error)
        };

        // Act
        var viewResult = _systemUnderTest.Index(statusCode);

        // Assert
        viewResult.ViewName.Should().Be(expectedPageName);

        if (statusCode == null)
        {
            _systemUnderTest.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }
        else
        {
            _systemUnderTest.Response.StatusCode.Should().Be(statusCode);
        }
    }
}