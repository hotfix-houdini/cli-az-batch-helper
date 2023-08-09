using System.Globalization;
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
        if (scheduleDoNotRunUntil != null && !DateTime.TryParseExact(scheduleDoNotRunUntil, "yyyy'-'MM'-'dd'T'HH':'mm':'ssK", null, DateTimeStyles.RoundtripKind, out _))
        {
            throw new ArgumentException($"Invalid ISO-8601 timestamp format", nameof(ScheduleDoNotRunUntil));
        }
        if (!string.IsNullOrWhiteSpace(jobManagerEnvironmentVariables))
        {
            var sanitizedEnvVars = jobManagerEnvironmentVariables
                .TrimEnd(',')
                .Replace("\r\n", "\n")
                .TrimEnd('\n');
            var containsComma = sanitizedEnvVars.Contains(',');
            var containsNewline = sanitizedEnvVars.Contains('\n');
            var kvpFromKvpDelimiter = containsNewline ? "\n" : ",";
            var keyFromValueDelimiter = "=";

            var keyValuePairs = sanitizedEnvVars.Split(kvpFromKvpDelimiter);
            if (keyValuePairs.Any(kvp => string.IsNullOrEmpty(kvp)))
            {
                throw new ArgumentException($"Invalid Environment Variables list - repeated delimiter detected", nameof(JobManagerEnvironmentVariables));
            }
            foreach (var kvp in keyValuePairs)
            {
                if (!kvp.Contains(keyFromValueDelimiter))
                {
                    throw new ArgumentException($"Invalid Environment Variables list - '{kvp}' does not have a value", nameof(JobManagerEnvironmentVariables));
                }
                var key = kvp.Split(keyFromValueDelimiter)[0];
                var value = kvp.Split(keyFromValueDelimiter)[1];
                if (string.IsNullOrWhiteSpace(key)) 
                { 
                    throw new ArgumentException($"Invalid Environment Variables list - no key for value '{kvp.Replace(keyFromValueDelimiter, "")}'", nameof(JobManagerEnvironmentVariables)); 
                }
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException($"Invalid Environment Variables list - '{kvp.Replace(keyFromValueDelimiter, "")}' does not have a value", nameof(JobManagerEnvironmentVariables));
                }
            }
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
