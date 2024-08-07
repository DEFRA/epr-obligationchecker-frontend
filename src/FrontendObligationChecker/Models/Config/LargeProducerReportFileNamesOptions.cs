namespace FrontendObligationChecker.Models.Config;

using System.ComponentModel.DataAnnotations;

public class LargeProducerReportFileNamesOptions
{
    public const string ConfigSection = "LargeProducerReportFileNames";

    [Required]
    public string EnglishReportFileName { get; set; }

    [Required]
    public string ScottishReportFileName { get; set; }

    [Required]
    public string WalesReportFileName { get; set; }

    [Required]
    public string NorthernIrelandReportFileName { get; set; }

    [Required]
    public string AllNationsReportFileName { get; set; }

    [Required]
    public string EnglishReportFileNameInWelsh { get; set; }

    [Required]
    public string ScottishReportFileNameInWelsh { get; set; }

    [Required]
    public string WalesReportFileNameInWelsh { get; set; }

    [Required]
    public string NorthernIrelandReportFileNameInWelsh { get; set; }

    [Required]
    public string AllNationsReportFileNameInWelsh { get; set; }

    [Required]
    public string LatestAllNationsReportFileNamePrefix { get; set; }

    [Required]
    public string LatestAllNationsReportFileNamePrefixInWelsh { get; set; }

    [Required]
    public string LatestAllNationsReportDownloadFileName { get; set; }

    [Required]
    public string LatestAllNationsReportDownloadFileNameInWelsh { get; set; }
}