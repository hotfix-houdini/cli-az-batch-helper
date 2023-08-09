namespace AzBatchHelper.Cli.Services.File;

public class FileService : IFileService
{
    public async Task WriteFile(FileInfo file, string contents)
    {
        await System.IO.File.WriteAllTextAsync(file.FullName, contents);
    }
}
