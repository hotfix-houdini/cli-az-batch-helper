using AzBatchHelper.Cli.Commands.Generate.ScheduledJobConfig;
using AzBatchHelper.Cli.Services.File;
using Moq;

namespace AzBatchHelper.Tests.Commands.Generate.ScheduledJobConfig
{
    public class ScheduledJobConfigHandlerTests
    {

        [Test]
        public async Task ShouldWriteToOutputFileAsync()
        {
            // arrange
            var fileService = new Mock<IFileService>();
            var handler = new ScheduledJobConfigHandler(fileService.Object);
            var file = new FileInfo("some/file/path.json");
            var settings = ScheduledJobConfigSettingsTestHelper.ConstructScheduledJobConfigSettings(outputFile: file);

            fileService
                .Setup(x => x.WriteFile(file, It.IsAny<string>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // act
            await handler.HandleAsync(settings);

            // assert
            fileService.Verify();
        }

        [Theory]
        [TestCase("basic-test")]
        [TestCase("env-var-csv")]
        [TestCase("env-var-singlepair")]
        [TestCase("env-var-twopair")]
        [TestCase("env-var-twopair-trailingcomma")]
        public async Task ShouldWriteExpectedFile(string testFileSlug)
        {
            // arrange
            var fileService = new Mock<IFileService>();
            var handler = new ScheduledJobConfigHandler(fileService.Object);
            var testFile = File.ReadAllText($"Commands/Generate/ScheduledJobConfig/HandlerTestCases/{testFileSlug}.test");
            var inputs = testFile.Split(TestCaseDelimiter)[0];
            var expectedOutput = testFile.Split(TestCaseDelimiter)[1];

            var writtenFileContents = "-1";
            fileService
                .Setup(x => x.WriteFile(It.IsAny<FileInfo>(), It.IsAny<string>()))
                .Returns((FileInfo fileInfo, string fileContents) =>
                {
                    writtenFileContents = fileContents;
                    return Task.CompletedTask;
                });

            var settings = LoadSettingsFromTestFormat(inputs);

            // act
            await handler.HandleAsync(settings);

            // assert
            Assert.That(writtenFileContents, Is.EqualTo(expectedOutput));
        }

        [Theory]
        [TestCase(null, null)]
        [TestCase("key=value", 1, $"key{SmallerTestCaseDelimiter}value")]
        [TestCase("key=value,key2=value2", 2, $"key{SmallerTestCaseDelimiter}value", $"key2{SmallerTestCaseDelimiter}value2")]
        [TestCase("key=value,key2=value2,", 2, $"key{SmallerTestCaseDelimiter}value", $"key2{SmallerTestCaseDelimiter}value2")]
        [TestCase(@"key=value
key2=value2", 2, $"key{SmallerTestCaseDelimiter}value", $"key2{SmallerTestCaseDelimiter}value2")]
        [TestCase("key=value\nkey2=value2", 2, $"key{SmallerTestCaseDelimiter}value", $"key2{SmallerTestCaseDelimiter}value2")]
        [TestCase("key=value\r\nkey2=value2", 2, $"key{SmallerTestCaseDelimiter}value", $"key2{SmallerTestCaseDelimiter}value2")]
        [TestCase("key=value\r\n", 1, $"key{SmallerTestCaseDelimiter}value")]
        [TestCase("key=value\n", 1, $"key{SmallerTestCaseDelimiter}value")]
        [TestCase("key=value\nkey2=value2 with a , comma.\nkey3=value3", 3, $"key{SmallerTestCaseDelimiter}value", $"key2{SmallerTestCaseDelimiter}value2 with a , comma.", $"key3{SmallerTestCaseDelimiter}value3")]
        [TestCase("key=value,,\nkey2=value2", 2, $"key{SmallerTestCaseDelimiter}value,,", $"key2{SmallerTestCaseDelimiter}value2")]
        [TestCase(@"KEY_VAULT_URL=https://some-vault-4212.vault.azure.net
BATCH_ACCOUNT_KEY_SECRET_NAME=test-batch-account-key
MANAGED_IDENTITY_RESOURCE_ID=/subscriptions/11111111-1111-1111-1111-11111111/resourceGroups/bicep-batch-test/providers/Microsoft.ManagedIdentity/userAssignedIdentities/test-managed-identity
IMAGE_NAME=job-execution-poc
IMAGE_TAG=1.0.0
IMAGE_REGISTRY=supertestregistry4billion.azurecr.io", 6,
            $"KEY_VAULT_URL{SmallerTestCaseDelimiter}https://some-vault-4212.vault.azure.net",
            $"BATCH_ACCOUNT_KEY_SECRET_NAME{SmallerTestCaseDelimiter}test-batch-account-key",
            $"MANAGED_IDENTITY_RESOURCE_ID{SmallerTestCaseDelimiter}/subscriptions/11111111-1111-1111-1111-11111111/resourceGroups/bicep-batch-test/providers/Microsoft.ManagedIdentity/userAssignedIdentities/test-managed-identity",
            $"IMAGE_NAME{SmallerTestCaseDelimiter}job-execution-poc",
            $"IMAGE_TAG{SmallerTestCaseDelimiter}1.0.0",
            $"IMAGE_REGISTRY{SmallerTestCaseDelimiter}supertestregistry4billion.azurecr.io")]
        [TestCase(@"key=value
                    key2=value2", 2, $"key{SmallerTestCaseDelimiter}value", $"key2{SmallerTestCaseDelimiter}value2")]
        public void GenerateEnvironmentSettingsShouldReturnTheExpectedPairs(string envVarString, int? expectedCount, params string[] kvps)
        {
            // arrange
            // act
            var environmentSettings = ScheduledJobConfigHandler.GenerateEnvironmentSettings(envVarString);

            // assert
            Assert.That(environmentSettings?.Count, Is.EqualTo(expectedCount));
            Assert.That(expectedCount ?? 0, Is.EqualTo(kvps.Length));
            Assert.Multiple(() =>
            {
                for (int i = 0; i < kvps.Length; i++)
                {
                    var expectedKey = kvps[i].Split(SmallerTestCaseDelimiter)[0];
                    var expectedValue = kvps[i].Split(SmallerTestCaseDelimiter)[1];
                    var actualKey = environmentSettings[i].name;
                    var actualValue = environmentSettings[i].value;

                    Assert.That(actualKey, Is.EqualTo(expectedKey));
                    Assert.That(actualValue, Is.EqualTo(expectedValue));
                }
            });            
        }

        private ScheduledJobConfigSettings LoadSettingsFromTestFormat(string testInput)
        {
            var entries = testInput.Split(Environment.NewLine);
            var scheduledJobId = entries[0].Split("=")[1];
            var poolId = entries[1].Split("=")[1];
            var recurrence = entries[2].Split("=")[1];
            var doNotRunUntil = entries[3].Replace("do-not-run-until=", "");
            var jobManagerCommandLine = entries[4].Split("=")[1];
            var jobManagerImageName = entries[5].Split("=")[1];
            var jobManagerEnvVars = entries[6].Replace("job-manager-envvars=", "");

            if (string.IsNullOrWhiteSpace(doNotRunUntil))
            {
                doNotRunUntil = null;
            }
            if (string.IsNullOrWhiteSpace(jobManagerCommandLine))
            {
                jobManagerEnvVars = null;
            }

            return new ScheduledJobConfigSettings(new FileInfo("file.json"), scheduledJobId, poolId, recurrence, doNotRunUntil, jobManagerCommandLine, jobManagerImageName, jobManagerEnvVars);
        }

        private static readonly string TestCaseDelimiter = Environment.NewLine + @"@@@@@@@@@@@@@@@@@@@@@@@@@
@@   CLI inputs above  @@
@@ Expected JSON below @@
@@@@@@@@@@@@@@@@@@@@@@@@@" + Environment.NewLine;

        private const string SmallerTestCaseDelimiter = "!@!";
    }

}
