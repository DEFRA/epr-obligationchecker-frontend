using AutoMapper;
using FluentAssertions;
using FrontendObligationChecker.Models.ObligationChecker;
using FrontendObligationChecker.Models.Session;
using FrontendObligationChecker.Services.Session;
using FrontendObligationChecker.Services.Session.Interfaces;
using FrontendObligationChecker.UnitTests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;

namespace FrontendObligationChecker.UnitTests.Services.Session;
[TestClass]
public class JourneySessionTests
{
    private const string JourneySessionKey = nameof(SessionJourney);

    private JourneySession _systemUnderTest;
    private Mock<IDistributedSession<SessionJourney>> _distributedSessionMock;
    private Mock<IMapper> _mapperMock;
    private Mock<ILogger<JourneySession>> _loggerMock;

    [TestInitialize]
    public void TestInitialise()
    {
        _distributedSessionMock = new Mock<IDistributedSession<SessionJourney>>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<JourneySession>>();
        _systemUnderTest = new JourneySession(_distributedSessionMock.Object, _mapperMock.Object, _loggerMock.Object);
    }

    [TestMethod]
    public async Task RemovePagesAfterCurrentPage_SetsCorrectSessionJourney()
    {
        // Arrange
        var sessionJourney = SessionHelper.GetSessionJourney();
        var currentPage = new Page { Index = 10 };
        var expectedPages = new List<SessionPage>
        {
            SessionHelper.GetOrganisationTypeSessionPage()
        };
        _distributedSessionMock.Setup(x => x.GetAsync(JourneySessionKey)).Returns(Task.FromResult(sessionJourney));

        // Act
        await _systemUnderTest.RemovePagesAfterCurrentAsync(currentPage);

        // Assert
        sessionJourney.Pages.Should().BeEquivalentTo(expectedPages);
        _distributedSessionMock.Verify(distributedSession => distributedSession.Remove(JourneySessionKey), Times.Once);
        _distributedSessionMock.Verify(distributedSession => distributedSession.SetAsync(JourneySessionKey, sessionJourney), Times.Once);
    }

    [TestMethod]
    public async Task RemovePagesAfterCurrentPage_CatchesAndLogsException_WhenExceptionIsThrown()
    {
        // Arrange
        var currentPage = new Page { Index = 1 };
        _distributedSessionMock.Setup(x => x.GetAsync(JourneySessionKey)).Throws<Exception>();

        // Act
        await _systemUnderTest.RemovePagesAfterCurrentAsync(currentPage);

        // Assert
        _loggerMock.VerifyLog(logger => logger.LogError(It.IsAny<Exception>(), "Error adding page to session"), Times.Once);
    }
}