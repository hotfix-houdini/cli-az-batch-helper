using AzBatchHelper.Cli.Commands.Generate.ScheduledJobConfig;

namespace AzBatchHelper.Tests.Commands.Generate.ScheduledJobConfig;

public static class ScheduledJobConfigSettingsTestHelper
{
    public const string DefaultFileName = "file.json";
    public const string DefaultScheduledJobId = "schedule-job-id";
    public const string DefaultPoolId = "pool-id";
    public const string DefaultRecurrence = "PT5M";
    public const string DefaultDoNoRunUntil = null;
    public const string DefaultJobManagerCommandLine = "\"\"";
    public const string DefaultImageName = "myregistry.azurecr.io/my-image:1.0.0";
    public const string DefaultEnvVars = null;

    public static ScheduledJobConfigSettings ConstructScheduledJobConfigSettings(
        FileInfo? outputFile = null,
        string scheduledJobId = DefaultScheduledJobId,
        string poolId = DefaultPoolId,
        string scheduleRecurrence = DefaultRecurrence,
        string? scheduleDoNotRunUntil = DefaultDoNoRunUntil,
        string jobManagerCommandLine = DefaultJobManagerCommandLine,
        string jobManagerImage = DefaultImageName,
        string? jobManagerEnvVars = DefaultEnvVars)
    {
        outputFile ??= new FileInfo(DefaultFileName);
        return new ScheduledJobConfigSettings(outputFile, scheduledJobId, poolId, scheduleRecurrence, scheduleDoNotRunUntil, jobManagerCommandLine, jobManagerImage, jobManagerEnvVars);
    }
}