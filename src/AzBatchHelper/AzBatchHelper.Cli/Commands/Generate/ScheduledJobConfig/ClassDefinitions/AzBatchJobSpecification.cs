namespace AzBatchHelper.Cli.Commands.Generate.ScheduledJobConfig.ClassDefinitions;

public class AzBatchJobSpecification
{
    public AzBatchJobManagerTask jobManagerTask { get; set; }
    public string onAllTasksComplete { get; set; }
    public AzBatchPoolInfo poolInfo { get; set; }
}