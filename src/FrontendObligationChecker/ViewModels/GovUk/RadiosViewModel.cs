using FrontendObligationChecker.Models.ObligationChecker;

using Microsoft.AspNetCore.Mvc.Localization;

namespace FrontendObligationChecker.ViewModels.GovUk;

public class RadiosViewModel
{
    public RadiosViewModel()
    {
    }

    public RadiosViewModel(Question question, IViewLocalizer localizer) : this()
    {
        Id = question.Key;
        Fieldset = new FieldsetViewModel
        {
            Legend = new LegendViewModel
            {
                IsPageHeading = false,
                Text = localizer[question.Title].Value,
                Classes = "govuk-!-margin-bottom-5"
            },
            DescribedBy = question.HasError ? $"{question.Key}-error" : null
        };
        Detail = question.HasDetail
            ? new DetailViewModel
            {
                Summary = localizer[question.Summary].Value,
                Detail = localizer[question.Detail].Value,
                Position = question.DetailPosition
            }
            : null;
        Items = question.Options.Select(option => new RadioItemViewModel
        {
            Id = $"{question.Key}-{option.Value}",
            IsSelected = option.IsSelected.GetValueOrDefault(),
            Label = new LabelViewModel
            {
                For = $"{question.Key}-{option.Value}", Text = localizer[option.Title].Value
            },
            Name = question.Key,
            Value = option.Value
        }).ToList();
        Hint = !string.IsNullOrEmpty(question.GetDescription())
            ? new HintViewModel
            {
                Id = $"{question.Key}-hint", Text = localizer[question.GetDescription()].Value
            }
            : null;
        Error = question.HasError
            ? new ErrorViewModel
            {
                Id = $"{question.Key}-error", Text = localizer[question.ErrorMessage].Value
            }
            : null;
    }

    public string Id { get; set; }

    public FieldsetViewModel? Fieldset { get; set; }

    public FormGroupViewModel? FormGroup { get; set; }

    public List<RadioItemViewModel> Items { get; set; } = default!;

    public HintViewModel? Hint { get; set; }

    public DetailViewModel? Detail { get; set; }

    public string? Classes { get; set; }

    public ErrorViewModel? Error { get; set; }

    public string? AdditionalDescription { get; set; }
}