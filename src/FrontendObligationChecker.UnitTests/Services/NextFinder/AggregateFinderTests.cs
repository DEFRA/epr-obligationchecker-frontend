using FluentAssertions;
using FrontendObligationChecker.Models.ObligationChecker;
using FrontendObligationChecker.Services.NextFinder;

namespace FrontendObligationChecker.UnitTests.Services.NextFinder;
[TestClass]
public class AggregateFinderTests
{
    private AggregateFinder? _systemUnderTest;

    [TestInitialize]
    public void Setup()
    {
        _systemUnderTest = new AggregateFinder();
    }

    [TestMethod]
    public void Next_OneTrue_ReturnsSecondaryPath()
    {
        // Arrange
        var questions = NextFinderHelper.GetMultiQuestionList_OneTrue();

        // Act
        var result = _systemUnderTest!.Next(questions);

        // Assert
        result.Should().Be(OptionPath.Secondary);
    }

    [TestMethod]
    public void Next_AllNo_ReturnsPrimaryPath()
    {
        // Arrange
        var questions = NextFinderHelper.GetMultiQuestionList_AllFalse();

        // Act
        var result = _systemUnderTest!.Next(questions);

        // Assert
        result.Should().Be(OptionPath.Primary);
    }

    [TestMethod]
    public void Next_NullQuestions_Throws()
    {
        // Arrange
        IList<Question> questions = null!;

        // Act
        Action act = () => _systemUnderTest!.Next(questions);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}