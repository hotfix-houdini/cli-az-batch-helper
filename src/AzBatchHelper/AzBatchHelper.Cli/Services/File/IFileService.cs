namespace AzBatchHelper.Cli.Services.File;

public interface IFileService
{
    Task WriteFile(FileInfo file, string contents);
}
