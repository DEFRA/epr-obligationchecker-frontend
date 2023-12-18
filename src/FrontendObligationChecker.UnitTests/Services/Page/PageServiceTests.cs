using FluentAssertions;

using FrontendObligationChecker.Generators;
using FrontendObligationChecker.Models.Config;
using FrontendObligationChecker.Models.ObligationChecker;
using FrontendObligationChecker.Models.Session;
using FrontendObligationChecker.Services.PageService.Interfaces;
using FrontendObligationChecker.Services.Session.Interfaces;
using FrontendObligationChecker.UnitTests.Helpers;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

using Moq;

namespace FrontendObligationChecker.UnitTests.Services.PageService;
[TestClass]
public class PageServiceTests
{
    private IPageService _systemUnderTest;
    private Mock<IJourneySession> _journeySessionMock;
    private Mock<IOptions<ExternalUrlsOptions>> _externalUrls;

    [TestInitialize]
    public void TestInitialize()
    {
        _journeySessionMock = new Mock<IJourneySession>();
        _externalUrls = new Mock<IOptions<ExternalUrlsOptions>>();

        _systemUnderTest = new FrontendObligationChecker.Services.PageService.PageService(_journeySessionMock.Object, _externalUrls.Object);
    }

    [TestMethod]
    public async Task GetPage_NotExistingPath_Throws()
    {
        // Arrange
        const string nonExistingPath = "NON_EXISTING_PATH";

        // Act
        var act = async () => await _systemUnderTest.GetPageAsync(nonExistingPath);

        // Assert
        act.Should().ThrowAsync<ArgumentNullException>();
    }

    [TestMethod]
    public async Task GetPage_DirectSetToBack_CallsSetAnswer()
    {
        // Arrange
        const string nonExistingPath = "NON_EXISTING_PATH";

        // Act
        var act = async () => await _systemUnderTest.GetPageAsync(nonExistingPath);

        // Assert
        act.Should().ThrowAsync<ArgumentNullException>();
    }

    [TestMethod]
    public async Task GetPage_ReturnsNull_WhenSessionIsNullAndPagePathIsNotOrganisationType()
    {
        // Arrange
        const string path = PagePath.AnnualTurnover;
        _journeySessionMock.Setup(x => x.GetAsync()).Returns(Task.FromResult<SessionJourney>(null));

        // Act
        var act = await _systemUnderTest.GetPageAsync(path);

        // Assert
        act.Should().BeNull();
    }

    [TestMethod]
    public async Task GetPage_DoesNotReturnNull_WhenSessionIsNullAndPagePathIsOrganisationType()
    {
        // Arrange
        const string path = PagePath.TypeOfOrganisation;
        _journeySessionMock.Setup(x => x.GetAsync()).Returns(Task.FromResult<SessionJourney>(null));

        // Act
        var act = await _systemUnderTest.GetPageAsync(path);

        // Assert
        act.Should().NotBeNull();
    }

    [TestMethod]
    public async Task GetPage_DoesNotReturnNull_WhenSessionIsNotNull()
    {
        // Arrange
        const string path = PagePath.TypeOfOrganisation;
        _journeySessionMock.Setup(x => x.GetAsync()).Returns(Task.FromResult(new SessionJourney()));

        // Act
        var act = await _systemUnderTest.GetPageAsync(path);

        // Assert
        act.Should().NotBeNull();
    }

    [TestMethod]
    public async Task GetPageSetAnswersAndGetPage_ReturnsNull_WhenSessionIsNullAndPagePathIsNotOrganisationType()
    {
        // Arrange
        const string path = PagePath.AnnualTurnover;
        _journeySessionMock.Setup(x => x.GetAsync()).Returns(Task.FromResult<SessionJourney>(null));

        // Act
        var act = await _systemUnderTest.SetAnswersAndGetPageAsync(path, FormCollection.Empty);

        // Assert
        act.Should().BeNull();
    }

    [TestMethod]
    public async Task GetPageSetAnswersAndGetPage_DoesNotReturnNull_WhenSessionIsNullAndPagePathIsOrganisationType()
    {
        // Arrange
        const string path = PagePath.TypeOfOrganisation;
        _journeySessionMock.Setup(x => x.GetAsync()).Returns(Task.FromResult<SessionJourney>(null));

        // Act
        var act = await _systemUnderTest.SetAnswersAndGetPageAsync(path, FormCollection.Empty);

        // Assert
        act.Should().NotBeNull();
    }

    [TestMethod]
    public async Task GetPageSetAnswersAndGetPage_DoesNotReturnNull_WhenSessionIsNotNull()
    {
        // Arrange
        const string path = PagePath.TypeOfOrganisation;
        _journeySessionMock.Setup(x => x.GetAsync()).Returns(Task.FromResult(new SessionJourney()));

        // Act
        var act = await _systemUnderTest.SetAnswersAndGetPageAsync(path, FormCollection.Empty);

        // Assert
        act.Should().NotBeNull();
    }

    [TestMethod]
    public async Task SetAnswersAndGetPageAsync_DoesNotCallRemovePagesAfterCurrentAsync_WhenSessionIsNull()
    {
        // Arrange
        const string path = PagePath.TypeOfOrganisation;
        var pages = PageGenerator.Create(string.Empty);

        _journeySessionMock.Setup(x => x.GetAsync()).Returns(Task.FromResult<SessionJourney>(null));

        // Act
        await _systemUnderTest.SetAnswersAndGetPageAsync(path, FormCollection.Empty);

        // Assert
        _journeySessionMock.Verify(x => x.RemovePagesAfterCurrentAsync(It.IsAny<Page>()), Times.Never);
    }

    [TestMethod]
    public async Task SetAnswersAndGetPageAsync_DoesNotCallRemovePagesAfterCurrentAsync_WhenAnswerIsSameAsAnswerStoredInSession()
    {
        // Arrange
        const string path = PagePath.TypeOfOrganisation;
        var pages = PageGenerator.Create(string.Empty);
        var formCollection = new FormCollection(new Dictionary<string, StringValues> { { QuestionKey.TypeOfOrganisation, "parent" } });
        _journeySessionMock.Setup(x => x.GetAsync()).Returns(Task.FromResult(SessionHelper.GetSessionJourney()));

        // Act
        await _systemUnderTest.SetAnswersAndGetPageAsync(path, formCollection);

        // Assert
        _journeySessionMock.Verify(x => x.RemovePagesAfterCurrentAsync(It.IsAny<Page>()), Times.Never);
    }

    [TestMethod]
    public async Task SetAnswersAndGetPageAsync_CallsRemovePagesAfterCurrentAsync_WhenAnswerDiffersFromAnswerStoredInSession()
    {
        // Arrange
        const string path = PagePath.TypeOfOrganisation;
        var pages = PageGenerator.Create(string.Empty);
        var formCollection = new FormCollection(new Dictionary<string, StringValues> { { QuestionKey.TypeOfOrganisation, "2" } });
        _journeySessionMock.Setup(x => x.GetAsync()).Returns(Task.FromResult(SessionHelper.GetSessionJourney()));

        // Act
        await _systemUnderTest.SetAnswersAndGetPageAsync(path, formCollection);

        // Assert
        _journeySessionMock.Verify(x => x.RemovePagesAfterCurrentAsync(It.IsAny<Page>()), Times.Once);
    }

    [TestMethod]
    public async Task SetAnswersAndGetPageAsync_DoNotAddAnswersToSession_WhenThereIsMissingAnswer()
    {
        // Arrange
        const string path = PagePath.TypeOfOrganisation;
        var pages = PageGenerator.Create(string.Empty);
        var formCollection = new FormCollection(new Dictionary<string, StringValues> { { QuestionKey.TypeOfOrganisation, "-" } });
        _journeySessionMock.Setup(x => x.GetAsync()).Returns(Task.FromResult(SessionHelper.GetSessionJourney()));

        // Act
        await _systemUnderTest.SetAnswersAndGetPageAsync(path, formCollection);

        // Assert
        _journeySessionMock.Verify(x => x.AddPageAsync(It.IsAny<Page>()), Times.Never);
    }

    [TestMethod]
    public async Task SetAnswersAndGetPageAsync_AddAnswersToSession_WhenAnswersValid()
    {
        // Arrange
        const string path = PagePath.TypeOfOrganisation;
        var pages = PageGenerator.Create(string.Empty);
        var formCollection = new FormCollection(new Dictionary<string, StringValues> { { QuestionKey.TypeOfOrganisation, "parent" } });
        _journeySessionMock.Setup(x => x.GetAsync()).Returns(Task.FromResult(SessionHelper.GetSessionJourney()));

        // Act
        await _systemUnderTest.SetAnswersAndGetPageAsync(path, formCollection);

        // Assert
        _journeySessionMock.Verify(x => x.AddPageAsync(It.IsAny<Page>()), Times.Once);
    }
}