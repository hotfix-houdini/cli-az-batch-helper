using System.Text.RegularExpressions;

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
        if (!Regex.IsMatch(scheduleRecurrence, @"^P(?:(\d+)Y)?(?:(\d+)M)?(?:(\d+)D)?(?:T(?:(\d+)H)?(?:(\d+)M)?(?:(\d+(?:[\.,]\d*)?)S)?)?$"))
        {
            throw new ArgumentException($"Invalid ISO-8601 duration format", nameof(ScheduleRecurrence));
        }

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
