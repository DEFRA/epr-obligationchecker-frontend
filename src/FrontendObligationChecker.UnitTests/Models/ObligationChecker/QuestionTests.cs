using FluentAssertions;
using FrontendObligationChecker.Models.ObligationChecker;

namespace FrontendObligationChecker.UnitTests.Models.ObligationChecker;
[TestClass]
public class QuestionTests
{
    [TestMethod]
    public void SetAnswer_EmptyAnswer_SetErrorToTrue()
    {
        // Arrange
        var question = new Question();
        question.SetAnswer(string.Empty);

        // Assert
        question.HasError.Should().BeTrue();
    }

    [TestMethod]
    public void SetAnswer_InvalidAnswer_SetErrorToTrue()
    {
        // Arrange
        var question = new Question();
        const string invalidAnswer = "invalid answer";

        question.SetAnswer(invalidAnswer);

        // Assert
        question.HasError.Should().BeTrue();
    }

    [TestMethod]
    public void SetAnswer_ValidAnswer_SetsErrorToFalse()
    {
        // Arrange
        const string validAnswer = "valid answer";
        var question = new Question()
        {
            Options = new List<Option>()
            {
                new Option() { Value = validAnswer }
            }
        };
        question.SetAnswer(validAnswer);

        // Assert
        question.HasError.Should().BeFalse();
    }
}