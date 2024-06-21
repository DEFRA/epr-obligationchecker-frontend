namespace FrontendObligationChecker.ViewModels.GovUk;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class FieldsetViewModel
{
    public string DescribedBy { get; set; } = default!;

    public LegendViewModel? Legend { get; set; }

    public string RadioClasses { get; set; } = default!;
}