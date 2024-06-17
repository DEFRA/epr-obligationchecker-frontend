using System.Diagnostics.CodeAnalysis;
using FrontendObligationChecker.Models.ObligationChecker;

namespace FrontendObligationChecker.ViewModels.GovUk;

[ExcludeFromCodeCoverage]
public class DetailViewModel
{
    public string Summary { get; set; } = default!;

    public string Detail { get; set; } = default!;

    public DetailPosition Position { get; set; }

    public string Classes { get; set; } = default!;
}