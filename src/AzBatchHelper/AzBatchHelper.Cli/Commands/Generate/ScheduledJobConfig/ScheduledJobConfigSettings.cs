namespace AzBatchHelper.Cli.Commands.Generate.ScheduledJobConfig;

public class ScheduledJobConfigSettings
{
    public FileInfo OutputFile { get; private set; }
    public string ScheduledJobId { get; private set; }
    public string PoolId { get; private set; }
    public string ScheduleRecurrence { get; private set; }
    public string? ScheduleDoNotRunUntil { get; private set; }
    public string JobManagerCommandLine { get; private set; }
    public string JobManagerImage { get; private set; }
    public string? JobManagerEnvironmentVariables { get; private set; }

    public ScheduledJobConfigSettings(FileInfo outputFile, string scheduledJobId, string poolId, string scheduleRecurrence, string? scheduleDoNotRunUntil, string jobManagerCommandLine, string jobManagerImage, string? jobManagerEnvironmentVariables)
    {
        OutputFile = outputFile;
        ScheduledJobId = scheduledJobId;
        PoolId = poolId;
        ScheduleRecurrence = scheduleRecurrence;
        ScheduleDoNotRunUntil = scheduleDoNotRunUntil;
        JobManagerCommandLine = jobManagerCommandLine;
        JobManagerImage = jobManagerImage;
        JobManagerEnvironmentVariables = jobManagerEnvironmentVariables;
    }
}
