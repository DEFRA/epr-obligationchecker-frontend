namespace FrontendObligationChecker.Models.Config;

using System.ComponentModel.DataAnnotations;

public class LargeProducerReportFileNamesOptions
{
    public const string ConfigSection = "LargeProducerReportFileNames";

    [Required]
    public string En { get; set; }

    [Required]
    public string Sc { get; set; }

    [Required]
    public string Wl { get; set; }

    [Required]
    public string Ni { get; set; }

    [Required]
    public string All { get; set; }
}