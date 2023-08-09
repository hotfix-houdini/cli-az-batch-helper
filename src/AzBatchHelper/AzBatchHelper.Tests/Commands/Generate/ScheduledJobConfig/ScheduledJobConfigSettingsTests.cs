using AzBatchHelper.Cli.Commands.Generate.ScheduledJobConfig;
using Moq;

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

        [Theory]
        [TestCase("PT5M", true)] // 5 minutes
        [TestCase("P1DT0H0M0S", true)] // 1 day
        [TestCase("P3Y6M4DT12H30M5S", true)]
        [TestCase("PT0S", true)]
        [TestCase("P0D", true)]
        [TestCase("P1M", true)]
        [TestCase("PT1M", true)]
        [TestCase("PT36H", true)]
        [TestCase("P1DT12H", true)]
        [TestCase("00:05:00", false, $"Invalid ISO-8601 duration format (Parameter '{nameof(ScheduledJobConfigSettings.ScheduleRecurrence)}')")]
        [TestCase("5 minutes", false, $"Invalid ISO-8601 duration format (Parameter '{nameof(ScheduledJobConfigSettings.ScheduleRecurrence)}')")]
        [TestCase("5", false, $"Invalid ISO-8601 duration format (Parameter '{nameof(ScheduledJobConfigSettings.ScheduleRecurrence)}')")]
        //[TestCase("P0,5Y", true)] // might be valid but can't find appropriate regex
        //[TestCase("P0.5Y", true)] // might be valid but can't find appropriate regex
        //[TestCase("P2W", true)] // might be valid but can't find appropriate regex
        public void ConstructionShouldValidateISO8601DurationForRecurrence(string recurrenceString, bool shouldPassValidation, string? expectedExceptionMessage = null)
        {
            // arrange 
            ScheduledJobConfigSettings? scheduledJobConfigSettings = null;
            Exception? potentialException = null;

            // act
            try
            {
                scheduledJobConfigSettings = ConstructScheduledJobConfigSettings(scheduleRecurrence: recurrenceString);
            }
            catch (Exception ex)
            {
                potentialException = ex;
            }

            // assert
            if (shouldPassValidation)
            {
                Assert.That(potentialException, Is.Null);
                Assert.That(scheduledJobConfigSettings, Is.Not.Null);
                Assert.That(scheduledJobConfigSettings?.ScheduleRecurrence, Is.EqualTo(recurrenceString));
            }
            else
            {
                Assert.That(potentialException, Is.Not.Null);
                Assert.That(potentialException, Is.TypeOf<ArgumentException>());
                Assert.That(potentialException?.Message, Is.EqualTo(expectedExceptionMessage));
            }
        }

        [Theory]
        [TestCase("2030-08-07T11:43:00+00:00", true)]
        [TestCase("2030-08-07", false, $"Invalid ISO-8601 timestamp format (Parameter '{nameof(ScheduledJobConfigSettings.ScheduleDoNotRunUntil)}')")]
        public void ConstructionShouldValidateISO8601TimestampForDoNotRunUntil(string doNotRunUntilString, bool shouldPassValidation, string? expectedExceptionMessage = null)
        {
            // arrange 
            ScheduledJobConfigSettings? scheduledJobConfigSettings = null;
            Exception? potentialException = null;

            // act
            try
            {
                scheduledJobConfigSettings = ConstructScheduledJobConfigSettings(scheduleDoNotRunUntil: doNotRunUntilString);
            }
            catch (Exception ex)
            {
                potentialException = ex;
            }

            // assert
            if (shouldPassValidation)
            {
                Assert.That(potentialException, Is.Null);
                Assert.That(scheduledJobConfigSettings, Is.Not.Null);
                Assert.That(scheduledJobConfigSettings?.ScheduleDoNotRunUntil, Is.EqualTo(doNotRunUntilString));
            }
            else
            {
                Assert.That(potentialException, Is.Not.Null);
                Assert.That(potentialException, Is.TypeOf<ArgumentException>());
                Assert.That(potentialException?.Message, Is.EqualTo(expectedExceptionMessage));
            }
        }


        // assert time ISO-8601 timestamp and duration compliance (doNotRunUntil, recurrence)
        // assert envs are key=value pairs (support comma seperated and new-line seperated)
        // env var validation
        // seperated value should be key=value
        // comma, no new line
        // new line, no comma
        // comma and new line
        // no comma and no new line



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
