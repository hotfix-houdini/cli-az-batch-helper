using AzBatchHelper.Cli.Commands.Generate.ScheduledJobConfig;

namespace AzBatchHelper.Tests.Commands.Generate.ScheduledJobConfig
{
    public class ScheduledJobConfigSettingsTests
    {
        private const string DefaultFileName = "file.json";
        private const string DefaultScheduledJobId = "schedule-job-id";
        private const string DefaultPoolId = "pool-id";
        private const string DefaultRecurrence = "PT5M";
        private const string DefaultDoNoRunUntil = null;
        private const string DefaultJobManagerCommandLine = "\"\"";
        private const string DefaultImageName = "myregistry.azurecr.io/my-image:1.0.0";
        private const string DefaultEnvVars = null;

        [Test]
        public void AcceptableSettingsShouldPassConstruction()
        {
            // arrange & act
            var settings = ConstructScheduledJobConfigSettings();

            // assert
            Assert.That(settings.OutputFile.Name, Is.EqualTo(DefaultFileName));
            Assert.That(settings.ScheduledJobId, Is.EqualTo(DefaultScheduledJobId));
            Assert.That(settings.PoolId, Is.EqualTo(DefaultPoolId));
            Assert.That(settings.ScheduleRecurrence, Is.EqualTo(DefaultRecurrence));
            Assert.That(settings.ScheduleDoNotRunUntil, Is.EqualTo(DefaultDoNoRunUntil));
            Assert.That(settings.JobManagerCommandLine, Is.EqualTo(DefaultJobManagerCommandLine));
            Assert.That(settings.JobManagerImage, Is.EqualTo(DefaultImageName));
            Assert.That(settings.JobManagerEnvironmentVariables, Is.EqualTo(DefaultEnvVars));
        }

        // add validation tests (should throw argument exception)

        public ScheduledJobConfigSettings ConstructScheduledJobConfigSettings(
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
}
