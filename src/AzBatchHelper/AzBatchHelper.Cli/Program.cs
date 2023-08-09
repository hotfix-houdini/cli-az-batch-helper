// See https://aka.ms/new-console-template for more information
using AzBatchHelper.Cli.Commands.Generate.ScheduledJobConfig;
using AzBatchHelper.Cli.Services.File;
using System.CommandLine;

var generateScheduledJobConfigCommand = new Command("scheduled-job-config", "Generate a scheduled job json file for creating a scheduled job in Azure Batch.");

var outputFileGenerateOption = new Option<FileInfo>(name: "--output", description: "The output file for the generated artifact.") { IsRequired = true };
var scheduledJobIdOption = new Option<string>(name: "--scheduled-job-id", description: "The scheduled job id.") { IsRequired = true };
var poolOption = new Option<string>(name: "--pool", description: "The pool id.") { IsRequired = true };
var scheduleRecurrenceOption = new Option<string>(name: "--schedule-recurrence", description: "The schedule recurrence. ISO-8601 duration. (Example PT5M)") { IsRequired = true };
var scheduledoNotRunUntilOption = new Option<string?>(name: "--schedule-do-not-run-until", description: "The schedule do not run until. ISO-8601 date time. (Example 2030-08-07T11:43:00+00:00)");
var jobManagerCommandLineOption = new Option<string>(name: "--job-manager-command-line", description: "The job manager command line.", getDefaultValue: () => "\"\"");
var jobManagerImageOption = new Option<string>(name: "--job-manager-image", description: "The job manager image.") { IsRequired = true };
var jobManagerEnvironmentVariablesOption = new Option<string?>(name: "--job-manager-env-vars", description: "Comma seperated or newline seperated list of key=value environment variables.");

generateScheduledJobConfigCommand.AddOption(outputFileGenerateOption);
generateScheduledJobConfigCommand.AddOption(scheduledJobIdOption);
generateScheduledJobConfigCommand.AddOption(poolOption);
generateScheduledJobConfigCommand.AddOption(scheduleRecurrenceOption);
generateScheduledJobConfigCommand.AddOption(scheduledoNotRunUntilOption);
generateScheduledJobConfigCommand.AddOption(jobManagerCommandLineOption);
generateScheduledJobConfigCommand.AddOption(jobManagerImageOption);
generateScheduledJobConfigCommand.AddOption(jobManagerEnvironmentVariablesOption);
generateScheduledJobConfigCommand.SetHandler((outputFile, scheduledJobId, poolId, scheduleRecurrence, scheduleDoNotRunUntil, jobManagerCommandLine, jobManagerImage, jobManagerEnvVars) =>
{
    var settings = new ScheduledJobConfigSettings(outputFile, scheduledJobId, poolId, scheduleRecurrence, scheduleDoNotRunUntil, jobManagerCommandLine, jobManagerImage, jobManagerEnvVars);
    var handler = new ScheduledJobConfigHandler(new FileService());
    return handler.HandleAsync(settings);
}, outputFileGenerateOption, scheduledJobIdOption, poolOption, scheduleRecurrenceOption, scheduledoNotRunUntilOption, jobManagerCommandLineOption, jobManagerImageOption, jobManagerEnvironmentVariablesOption);

var generateCommand = new Command("generate", description: "Generate an artifact (such as a scheduled job .json file) to aid in Azure Batch configuration.");
generateCommand.AddCommand(generateScheduledJobConfigCommand);

var rootCommand = new RootCommand("Azure Batch Helper CLI");
rootCommand.AddCommand(generateCommand);

await rootCommand.InvokeAsync(args);