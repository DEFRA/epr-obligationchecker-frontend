namespace FrontendObligationChecker.ViewModels.GovUk;

public class FieldsetViewModel
{
    public string DescribedBy { get; set; } = default!;

    public LegendViewModel? Legend { get; set; }

    public string RadioClasses { get; set; } = default!;
}