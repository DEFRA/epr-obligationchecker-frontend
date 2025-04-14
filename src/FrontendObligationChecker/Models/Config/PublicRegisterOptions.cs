﻿namespace FrontendObligationChecker.Models.Config;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class PublicRegisterOptions
{
    public const string ConfigSection = "PublicRegister";

    public string? PublicRegisterBlobContainerName { get; set; }

    public string? PublicRegisterCsoBlobContainerName { get; set; }

    public DateTime PublishedDate { get; set; }
}