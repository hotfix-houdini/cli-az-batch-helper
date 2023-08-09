using AzBatchHelper.Cli.Commands.Generate.ScheduledJobConfig;

namespace AzBatchHelper.Tests.Commands.Generate.ScheduledJobConfig
{
    public class ScheduledJobConfigSettingsTests
    {
        [Test]
        public void AcceptableSettingsShouldPassConstruction()
        {
            // arrange & act
            var settings = ScheduledJobConfigSettingsTestHelper.ConstructScheduledJobConfigSettings();

            // assert
            Assert.That(settings.OutputFile.Name, Is.EqualTo(ScheduledJobConfigSettingsTestHelper.DefaultFileName));
            Assert.That(settings.ScheduledJobId, Is.EqualTo(ScheduledJobConfigSettingsTestHelper.DefaultScheduledJobId));
            Assert.That(settings.PoolId, Is.EqualTo(ScheduledJobConfigSettingsTestHelper.DefaultPoolId));
            Assert.That(settings.ScheduleRecurrence, Is.EqualTo(ScheduledJobConfigSettingsTestHelper.DefaultRecurrence));
            Assert.That(settings.ScheduleDoNotRunUntil, Is.EqualTo(ScheduledJobConfigSettingsTestHelper.DefaultDoNoRunUntil));
            Assert.That(settings.JobManagerCommandLine, Is.EqualTo(ScheduledJobConfigSettingsTestHelper.DefaultJobManagerCommandLine));
            Assert.That(settings.JobManagerImage, Is.EqualTo(ScheduledJobConfigSettingsTestHelper.DefaultImageName));
            Assert.That(settings.JobManagerEnvironmentVariables, Is.EqualTo(ScheduledJobConfigSettingsTestHelper.DefaultEnvVars));
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
                scheduledJobConfigSettings = ScheduledJobConfigSettingsTestHelper.ConstructScheduledJobConfigSettings(scheduleRecurrence: recurrenceString);
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
                scheduledJobConfigSettings = ScheduledJobConfigSettingsTestHelper.ConstructScheduledJobConfigSettings(scheduleDoNotRunUntil: doNotRunUntilString);
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

        [Theory]
        [TestCase(null, true)]
        [TestCase("key=value", true)]
        [TestCase("key=value,key2=value2", true)]
        [TestCase("key=value,key2=value2,", true)]
        [TestCase(@"key=value
                    key2=value2", true)]
        [TestCase("key=value\nkey2=value2", true)]
        [TestCase("key=value\r\nkey2=value2", true)]
        [TestCase("key=value\r\n", true)]
        [TestCase("key=value\n", true)]
        [TestCase("key=value\nkey2=value2 with a , comma.\nkey3=value3", true)]
        [TestCase("key=value,,\nkey2=value2", true)]
        [TestCase("key=value,,key2=value2", false, $"Invalid Environment Variables list - repeated delimiter detected (Parameter '{nameof(ScheduledJobConfigSettings.JobManagerEnvironmentVariables)}')")]
        [TestCase("key2", false, $"Invalid Environment Variables list - 'key2' does not have a value (Parameter '{nameof(ScheduledJobConfigSettings.JobManagerEnvironmentVariables)}')")]
        [TestCase("key=", false, $"Invalid Environment Variables list - 'key' does not have a value (Parameter '{nameof(ScheduledJobConfigSettings.JobManagerEnvironmentVariables)}')")]
        [TestCase("=value42", false, $"Invalid Environment Variables list - no key for value 'value42' (Parameter '{nameof(ScheduledJobConfigSettings.JobManagerEnvironmentVariables)}')")]
        [TestCase("KEY_VAULT_URL=https://some-vault-4212.vault.azure.net,BATCH_ACCOUNT_KEY_SECRET_NAME=test-batch-account-key,MANAGED_IDENTITY_RESOURCE_ID=/subscriptions/11111111-1111-1111-1111-11111111/resourceGroups/bicep-batch-test/providers/Microsoft.ManagedIdentity/userAssignedIdentities/test-managed-identity,IMAGE_NAME=job-execution-poc,IMAGE_TAG=1.0.0,IMAGE_REGISTRY=supertestregistry4billion.azurecr.io", true)]
        [TestCase(@"KEY_VAULT_URL=https://some-vault-4212.vault.azure.net
BATCH_ACCOUNT_KEY_SECRET_NAME=test-batch-account-key
MANAGED_IDENTITY_RESOURCE_ID=/subscriptions/11111111-1111-1111-1111-11111111/resourceGroups/bicep-batch-test/providers/Microsoft.ManagedIdentity/userAssignedIdentities/test-managed-identity
IMAGE_NAME=job-execution-poc
IMAGE_TAG=1.0.0
IMAGE_REGISTRY=supertestregistry4billion.azurecr.io", true)]
        public void ConstructionShouldValidateEnvironmentVariables(string envVarString, bool shouldPassValidation, string? expectedExceptionMessage = null)
        {
            // arrange 
            ScheduledJobConfigSettings? scheduledJobConfigSettings = null;
            Exception? potentialException = null;

            // act
            try
            {
                scheduledJobConfigSettings = ScheduledJobConfigSettingsTestHelper.ConstructScheduledJobConfigSettings(jobManagerEnvVars: envVarString);
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
                Assert.That(scheduledJobConfigSettings?.JobManagerEnvironmentVariables, Is.EqualTo(envVarString));
            }
            else
            {
                Assert.That(potentialException, Is.Not.Null);
                Assert.That(potentialException, Is.TypeOf<ArgumentException>());
                Assert.That(potentialException?.Message, Is.EqualTo(expectedExceptionMessage));
            }
        }
    }
}
