namespace AzBatchHelper.Cli.Commands.Generate.ScheduledJobConfig.ClassDefinitions;

public class AzBatchScheduledJob
{
    public string id { get; set; }
    public AzBatchJobSpecification jobSpecification { get; set; }
    public AzBatchSchedule schedule { get; set; }
}