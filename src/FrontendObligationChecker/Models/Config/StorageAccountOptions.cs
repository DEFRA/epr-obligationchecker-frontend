namespace FrontendObligationChecker.Models.Config;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class StorageAccountOptions
{
    public const string ConfigSection = "StorageAccount";

    [Required]
    public string ConnectionString { get; set; }

    [Required]
    public string BlobContainerName { get; set; }
}