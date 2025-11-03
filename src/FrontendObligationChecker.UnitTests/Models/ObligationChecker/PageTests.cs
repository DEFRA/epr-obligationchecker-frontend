using FluentAssertions;
using FrontendObligationChecker.Models.ObligationChecker;
using FrontendObligationChecker.Services.NextFinder;
using FrontendObligationChecker.Services.NextFinder.Interfaces;
using Moq;

namespace FrontendObligationChecker.UnitTests.Models.ObligationChecker;
[TestClass]
public class PageTests
{
    private Mock<INextFinder>? _nextFinderMock;

    [TestInitialize]
    public void TestInitialize()
    {
        _nextFinderMock = new Mock<INextFinder>();
    }

    [TestMethod]
    public void Constructor_Sets_OptionFinder()
    {
        // Arrange
        var page = new Page();

        // Assert
        page.NextFinder.Should().NotBeNull();
        page.NextFinder.Should().BeOfType(typeof(OptionFinder));
    }

    [TestMethod]
    public void HasError_Returns_True_When_Question_Has_Error()
    {
        // Arrange
        var page = new Page();
        page.Questions.Add(new Question { HasError = true });

        // Assert
        page.HasError.Should().BeTrue();
    }

    [TestMethod]
    public void Errors_list_has_only_details_of_Questions_having_HasError_true()
    {
        var page = new Page()
        {
            Questions = new List<Question>
            {
                new()
                {
                    HasError = false
                }
            }
        };

        Assert.IsEmpty(page.Errors);
    }

    [TestMethod]
    public void Errors_list_has_the_same_count_as_Questions_when_all_Questions_are_HasError_true()
    {
        var page = new Page();

        const int errorCount = 10;

        for (int i = 0; i < errorCount; ++i)
        {
            page.Questions.Add(new Question
            {
                Key = $"key-{i}",
                ErrorMessage = "error message",
                HasError = true
            });
        }

        Assert.HasCount(errorCount, page.Errors);
    }

    [TestMethod]
    public void AdditionalDescription_WithPreviousPageAndMatchingKey_ReturnsAdditionalDescription()
    {
        // Arrange
        var page = new Page();
        var previousPage = new Page();

        previousPage.AdditionalDescriptions.Add(OptionPath.Primary, "Description1");
        previousPage.AdditionalDescriptions.Add(OptionPath.Secondary, "Description2");
        page.PreviousPage = previousPage;

        page.AdditionalDescriptions.Add(OptionPath.Primary, "Description1");
        page.AdditionalDescriptions.Add(OptionPath.Secondary, "Description2");

        // Act
        var result = page.AdditionalDescription;

        // Assert
        Assert.AreEqual("Description1", result);
    }

    [TestMethod]
    public void AdditionalDescription_WithPreviousPageAndNoMatchingKey_ReturnsNull()
    {
        // Arrange
        var page = new Page();
        var previousPage = new Page();

        previousPage.AdditionalDescriptions.Add(OptionPath.Primary, "Description1");
        previousPage.AdditionalDescriptions.Add(OptionPath.Secondary, "Description2");
        page.PreviousPage = previousPage;

        // Act
        var result = page.AdditionalDescription;

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void AdditionalDescription_WithNoPreviousPage_ReturnsNull()
    {
        // Arrange
        var page = new Page();

        // Act
        var result = page.AdditionalDescription;

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Title_WithPreviousPageAndMatchingKey_ReturnsTitles()
    {
        // Arrange
        var page = new Page();
        var previousPage = new Page();

        previousPage.Titles.Add(OptionPath.Primary, "Description1");
        previousPage.Titles.Add(OptionPath.Secondary, "Description2");
        page.PreviousPage = previousPage;

        page.Titles.Add(OptionPath.Primary, "Description1");
        page.Titles.Add(OptionPath.Secondary, "Description2");

        // Act
        var result = page.Title;

        // Assert
        Assert.AreEqual("Description1", result);
    }

    [TestMethod]
    public void Title_WithPreviousPageAndNoMatchingKey_ReturnsNull()
    {
        // Arrange
        var page = new Page();
        var previousPage = new Page();

        previousPage.Titles.Add(OptionPath.Primary, "Description1");
        previousPage.Titles.Add(OptionPath.Secondary, "Description2");
        page.PreviousPage = previousPage;

        // Act
        var result = page.Title;

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Title_WithNoPreviousPage_ReturnsFirstTitle()
    {
        // Arrange
        var page = new Page();
        page.Titles.Add(OptionPath.Primary, "Description1");
        page.Titles.Add(OptionPath.Secondary, "Description2");

        // Act
        var result = page.Title;

        // Assert
        Assert.AreEqual("Description1", result);
    }

    [TestMethod]
    public void GetAlternateTitles_ReturnsCorrectTitles()
    {
        // Arrange
        var page = new Page();
        var question1 = new Question { Answer = YesNo.Yes, AlternateTitle = "Title 1" };
        var question2 = new Question { Answer = YesNo.Yes, AlternateTitle = "Title 2" };
        var question3 = new Question { Answer = YesNo.No, AlternateTitle = "Title 3" };

        var previousPage = new Page();
        previousPage.Questions.Add(question1);
        previousPage.Questions.Add(question2);
        previousPage.Questions.Add(question3);

        page.PreviousPage = previousPage;

        // Act
        var alternateTitles = page.GetAlternateTitles();

        // Assert
        Assert.HasCount(2, alternateTitles);
        Assert.Contains("Title 1", alternateTitles);
        Assert.Contains("Title 2", alternateTitles);
        Assert.DoesNotContain("Title 3", alternateTitles);
    }

}