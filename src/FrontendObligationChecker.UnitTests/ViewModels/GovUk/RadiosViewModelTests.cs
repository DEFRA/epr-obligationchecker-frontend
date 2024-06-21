namespace FrontendObligationChecker.UnitTests.ViewModels.GovUk;

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using FrontendObligationChecker.Models.ObligationChecker;
using FrontendObligationChecker.ViewModels.GovUk;
using Microsoft.AspNetCore.Mvc.Localization;
using Moq;

[TestClass]
public class RadiosViewModelTests
{
    private List<Option> _options;
    private Question _question;

    private RadiosViewModel _radiosViewModel;

    [TestInitialize]
    public void TestInitialize()
    {
        _options = new List<Option>
        {
            new()
            {
                Title = "Option 1",
                Value = "Yes",
            }
        };

        _question = new Question
        {
            Key = "QuestionKey",
            Description = "Question description",
            Title = "Question title",
            Summary = "Question summary",
            Detail = "Question detail",
            DetailPosition = DetailPosition.AboveQuestion,
            HasError = true,
            ErrorMessage = "Error message",
            Options = _options
        };

        var localizer = new Mock<IViewLocalizer>();
        localizer.SetupGet(x => x[_question.Title]).Returns(new LocalizedHtmlString("Title", _question.Title));
        localizer.SetupGet(x => x[_question.Summary]).Returns(new LocalizedHtmlString("Summary", _question.Summary));
        localizer.SetupGet(x => x[_question.Detail]).Returns(new LocalizedHtmlString("Detail", _question.Detail));
        localizer.SetupGet(x => x[_question.Description]).Returns(new LocalizedHtmlString("Description", _question.Description));
        localizer.SetupGet(x => x[_question.ErrorMessage]).Returns(new LocalizedHtmlString("ErrorMessage", _question.ErrorMessage));
        localizer.SetupGet(x => x[_question.Options[0].Title]).Returns(new LocalizedHtmlString("Option1", _question.Options[0].Title));

        _radiosViewModel = new RadiosViewModel(_question, localizer.Object);
    }

    [TestMethod]
    public async Task Constructor_Sets_IndividualProperties()
    {
        _radiosViewModel.Id.Should().Be(_question.Key);
        _radiosViewModel.AdditionalDescription.Should().BeNull();
    }

    [TestMethod]
    public async Task Constructor_Sets_Fieldset()
    {
        _radiosViewModel.Fieldset.Should().BeEquivalentTo(new FieldsetViewModel
        {
            Legend = new LegendViewModel
            {
                IsPageHeading = false,
                Text = _question.Title,
                Classes = "govuk-!-margin-bottom-5"
            },
            DescribedBy = $"{_question.Key}-error"
        });
    }

    [TestMethod]
    public async Task Constructor_Sets_Detail()
    {
        _radiosViewModel.Detail.Should().BeEquivalentTo(new DetailViewModel
        {
            Summary = _question.Summary,
            Detail = _question.Detail,
            Position = _question.DetailPosition
        });
    }

    [TestMethod]
    public async Task Constructor_Sets_Items()
    {
        _radiosViewModel.Items.Should().NotBeNullOrEmpty();
        _radiosViewModel.Items[0].Should().BeEquivalentTo(new RadioItemViewModel
        {
            Id = $"{_question.Key}-{_options[0].Value}",
            IsSelected = _options[0].IsSelected.GetValueOrDefault(),
            Label = new LabelViewModel
            {
                For = $"{_question.Key}-{_options[0].Value}",
                Text = _options[0].Title
            },
            Name = _question.Key,
            Value = _options[0].Value
        });
    }

    [TestMethod]
    public async Task Constructor_Sets_Hint()
    {
        _radiosViewModel.Hint.Should().BeEquivalentTo(new HintViewModel
        {
            Id = $"{_question.Key}-hint",
            Text = _question.Description
        });
    }

    [TestMethod]
    public async Task Constructor_Sets_Error()
    {
        _radiosViewModel.Error.Should().BeEquivalentTo(new ErrorViewModel
        {
            Id = $"{_question.Key}-error",
            Text = _question.ErrorMessage
        });
    }
}