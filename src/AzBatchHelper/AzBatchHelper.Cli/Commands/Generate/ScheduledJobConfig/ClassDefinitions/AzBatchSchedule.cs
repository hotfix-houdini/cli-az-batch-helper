namespace AzBatchHelper.Cli.Commands.Generate.ScheduledJobConfig.ClassDefinitions;

public class AzBatchSchedule
{
    public string recurrenceInterval { get; set; }
    public string? doNotRunUntil { get; set; }
}