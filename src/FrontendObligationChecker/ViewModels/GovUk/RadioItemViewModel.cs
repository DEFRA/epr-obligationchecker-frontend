namespace FrontendObligationChecker.ViewModels.GovUk;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class RadioItemViewModel
{
    public string Id { get; set; } = default!;

    public string Name { get; set; } = default!;

    public string Value { get; set; } = default!;

    public bool IsSelected { get; set; }

    public LabelViewModel Label { get; set; } = default!;
}