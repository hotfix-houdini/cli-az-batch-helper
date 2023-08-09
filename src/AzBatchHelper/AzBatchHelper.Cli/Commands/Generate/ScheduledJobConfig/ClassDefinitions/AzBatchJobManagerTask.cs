namespace AzBatchHelper.Cli.Commands.Generate.ScheduledJobConfig.ClassDefinitions;

public class AzBatchJobManagerTask
{
    public string commandLine { get; set; }
    public AzBatchContainerSettings containerSettings { get; set; }
    public List<AzBatchEnvironmentSetting>? environmentSettings { get; set; }
    public string id { get; set; }
    public bool killJobOnCompletion { get; set; }
}