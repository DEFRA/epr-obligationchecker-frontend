namespace FrontendObligationChecker.Models.ObligationChecker;

public class Option
{
    public string Title { get; set; } = default!;

    public string Value { get; set; } = default!;

    public bool? IsSelected { get; set; }

    public OptionPath Next { get; set; }
}