using FluentAssertions;
using FrontendObligationChecker.Models.ObligationChecker;
using FrontendObligationChecker.Services.NextFinder;

namespace FrontendObligationChecker.UnitTests.Services.NextFinder;
[TestClass]
public class OptionFinderTests
{
    private OptionFinder? _systemUnderTest;

    [TestInitialize]
    public void TestInitialize()
    {
        _systemUnderTest = new OptionFinder();
    }

    [TestMethod]
    public void Next_SingleQuestionList_ReturnsValidValue()
    {
        // Arrange
        var questions = NextFinderHelper.GetSingleQuestionList();

        // Act
        var result = _systemUnderTest!.Next(questions);

        // Assert
        result.Should().Be(OptionPath.Primary);
    }

    [TestMethod]
    public void Next_MultiQuestionList_Throw()
    {
        // Arrange
        var questions = NextFinderHelper.GetMultiQuestionList_OneTrue();

        // Act
        Action act = () => _systemUnderTest!.Next(questions);

        // Assert
        act.Should().Throw<InvalidOperationException>();
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

    [TestMethod]
    public void Next_is_null_when_no_questions_were_answered()
    {
        var questions = new List<Question>
        {
            new()
            {
                Options = new List<Option>
                {
                    new()
                }
            }
        };

        // Act
        OptionPath? path = _systemUnderTest.Next(questions);

        Assert.IsNull(path);
    }

    [TestMethod]
    public void Next_is_null_when_there_is_no_question()
    {
        var questions = new List<Question>();

        // Act
        OptionPath? path = _systemUnderTest.Next(questions);

        Assert.IsNull(path);
    }
}