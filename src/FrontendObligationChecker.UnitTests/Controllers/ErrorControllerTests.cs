namespace FrontendObligationChecker.UnitTests.Controllers;

using System.Net;
using FluentAssertions;
using FrontendObligationChecker.Controllers;
using FrontendObligationChecker.Models.ObligationChecker;

[TestClass]
public class ErrorControllerTests
{
    private readonly ErrorController _systemUnderTest = new();

    [TestMethod]
    [DataRow(HttpStatusCode.NotFound)]
    [DataRow(HttpStatusCode.InternalServerError)]
    [DataRow(HttpStatusCode.BadRequest)]
    public void Error_ReturnsErrorView_WhenCalledWithErrorCode(int statusCode)
    {
        // Arrange / Act
        var viewResult = _systemUnderTest.Error(statusCode);

        // Assert
        viewResult.ViewName.Should().Be(nameof(PageType.Error));
    }
}