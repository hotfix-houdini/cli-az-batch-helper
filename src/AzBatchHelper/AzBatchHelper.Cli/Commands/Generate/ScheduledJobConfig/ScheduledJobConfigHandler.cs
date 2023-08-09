using AzBatchHelper.Cli.Commands.Generate.ScheduledJobConfig.ClassDefinitions;
using AzBatchHelper.Cli.Services.File;
using System.Text.Json;

namespace AzBatchHelper.Cli.Commands.Generate.ScheduledJobConfig;

public class ScheduledJobConfigHandler
{
    private readonly IFileService _fileService;

    public ScheduledJobConfigHandler(IFileService fileService)
    {
        _fileService = fileService;
    }

    public Task HandleAsync(ScheduledJobConfigSettings settings)
    {
        var scheduledJob = new AzBatchScheduledJob()
        {
            id = settings.ScheduledJobId,
            schedule = new AzBatchSchedule()
            {
                recurrenceInterval = settings.ScheduleRecurrence,
                doNotRunUntil = settings.ScheduleDoNotRunUntil
            },
            jobSpecification = new AzBatchJobSpecification()
            {
                poolInfo = new AzBatchPoolInfo()
                {
                    autoPoolSpecification = null,
                    poolId = settings.PoolId
                },
                onAllTasksComplete = "terminatejob",
                jobManagerTask = new AzBatchJobManagerTask()
                {
                    id = "job-manager-task",
                    commandLine = settings.JobManagerCommandLine,
                    containerSettings = new AzBatchContainerSettings()
                    {
                        imageName = settings.JobManagerImage
                    },
                    environmentSettings = GenerateEnvironmentSettings(settings?.JobManagerEnvironmentVariables)
                }
            }
        };

        var scheduledJobJson = JsonSerializer.Serialize(scheduledJob, new JsonSerializerOptions()
        {
            WriteIndented = true,
        });

        _fileService.WriteFile(settings.OutputFile, scheduledJobJson);
        return Task.CompletedTask;
    }

    public static List<AzBatchEnvironmentSetting>? GenerateEnvironmentSettings(string? envVars)
    {
        if (string.IsNullOrWhiteSpace(envVars))
        {
            return null;
        }

        var sanitizedEnvVars = envVars
                .TrimEnd(',')
                .Replace("\r\n", "\n")
                .TrimEnd('\n');
        var containsNewline = sanitizedEnvVars.Contains('\n');
        var kvpFromKvpDelimiter = containsNewline ? "\n" : ",";
        var keyFromValueDelimiter = "=";
        var keyValuePairs = sanitizedEnvVars.Split(kvpFromKvpDelimiter);

        if (keyValuePairs.Length == 0)
        {
            return null;
        }

        var envVarEntries = new List<AzBatchEnvironmentSetting>();
        foreach (var kvp in keyValuePairs)
        {
            var key = kvp.Split(keyFromValueDelimiter)[0].TrimStart();
            var value = kvp.Split(keyFromValueDelimiter)[1];
            envVarEntries.Add(new AzBatchEnvironmentSetting()
            {
                name = key,
                value = value
            });
        }

        return envVarEntries;
    }
}
