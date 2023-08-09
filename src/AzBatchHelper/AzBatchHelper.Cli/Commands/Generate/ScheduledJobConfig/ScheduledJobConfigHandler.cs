using AzBatchHelper.Cli.Services.File;

namespace AzBatchHelper.Cli.Commands.Generate.ScheduledJobConfig;

public class ScheduledJobConfigHandler
{
    private readonly IFileService _fileService;

    public ScheduledJobConfigHandler(IFileService fileService)
    {
        _fileService = fileService;
    }

    public Task Handle(ScheduledJobConfigSettings settings)
    {
        // validate (have a validator service/class)
        // generate (have a settings<->string class)
        // write file (fileservice)
        throw new NotImplementedException();
    }
}
