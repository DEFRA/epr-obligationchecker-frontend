namespace FrontendObligationChecker.ViewModels.GovUk;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class CheckboxesNoneViewModel
{
    public string Id { get; set; }

    public FieldsetViewModel? Fieldset { get; set; }

    public FormGroupViewModel? FormGroup { get; set; }

    public List<RadioItemViewModel> Items { get; set; } = default!;

    public RadioItemViewModel NoneItem { get; set; } = default!;

    public HintViewModel? Hint { get; set; }

    public DetailViewModel? Detail { get; set; }

    public string? Classes { get; set; }

    public ErrorViewModel? Error { get; set; }

    public string? AdditionalDescription { get; set; }

    public string? DividerText { get; set; }
}