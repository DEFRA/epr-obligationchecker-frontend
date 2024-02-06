namespace FrontendObligationChecker.Readers;

public interface IBlobReader
{
    Task<Stream> DownloadBlobToStreamAsync(string fileName);
}