namespace FrontendObligationChecker.UnitTests.Controllers;

using System.Net;
using FluentAssertions;
using FrontendObligationChecker.Controllers;
using FrontendObligationChecker.Models.ObligationChecker;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

[TestClass]
public class ErrorControllerTests
{
    private readonly ErrorController _systemUnderTest;

    public ErrorControllerTests()
    {
        var logger = new Mock<ILogger<ErrorController>>(MockBehavior.Loose);

        _systemUnderTest = new ErrorController(logger.Object);
    }

    [TestMethod]
    [DataRow(HttpStatusCode.InternalServerError)]
    [DataRow(HttpStatusCode.BadRequest)]
    public void Error_ReturnsErrorView_WhenCalledWithErrorCode(HttpStatusCode statusCode)
    {
        // Arrange
        var httpContext = new DefaultHttpContext { Response = { StatusCode = (int)statusCode } };
        httpContext.Features.Set<IExceptionHandlerFeature>(null);
        httpContext.Features.Set<IStatusCodeReExecuteFeature>(null);

        _systemUnderTest.ControllerContext = new ControllerContext { HttpContext = httpContext };

        // Act
        var viewResult = _systemUnderTest.Error((int)statusCode);

        // Assert
        viewResult.ViewName.Should().Be(nameof(PageType.Error));
    }

    [TestMethod]
    public void Error_ReturnsErrorView_WhenCalledWithNotFound()
    {
        // Arrange
        var httpContext = new DefaultHttpContext { Response = { StatusCode = (int)HttpStatusCode.NotFound } };
        httpContext.Features.Set<IExceptionHandlerFeature>(null);
        httpContext.Features.Set<IStatusCodeReExecuteFeature>(new StatusCodeReExecuteFeature
        {
            OriginalQueryString = "?value=test",
            OriginalPath = "not/found"
        });

        _systemUnderTest.ControllerContext = new ControllerContext { HttpContext = httpContext };

        // Act
        var viewResult = _systemUnderTest.Error((int)HttpStatusCode.NotFound);

        // Assert
        viewResult.ViewName.Should().Be(nameof(PageType.Error));
    }

    [TestMethod]
    public void Error_ReturnsErrorView_WhenUnhandledExceptionThrown()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.Features.Set<IExceptionHandlerFeature>(new ExceptionHandlerFeature
        {
            Error = new ArgumentNullException(),
            Path = "error/path"
        });
        httpContext.Features.Set<IStatusCodeReExecuteFeature>(null);

        _systemUnderTest.ControllerContext = new ControllerContext { HttpContext = httpContext };

        // Act
        var viewResult = _systemUnderTest.Error((int)HttpStatusCode.InternalServerError);

        // Assert
        viewResult.ViewName.Should().Be(nameof(PageType.Error));
    }
}