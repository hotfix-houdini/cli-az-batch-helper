[![ci-dotnet](https://github.com/hotfix-houdini/cli-az-batch-helper/actions/workflows/ci-dotnet.yml/badge.svg)](https://github.com/hotfix-houdini/cli-az-batch-helper/actions/workflows/ci-dotnet.yml)

- [Overview](#overview)
- [Example CLI Calls](#example-cli-calls)
  * [Windows](#windows)
  * [Linux](#linux)
- [Running in a GHA Workflow](#running-in-a-gha-workflow)

# Overview
A Self Contained .NET 7 CLI that generates a JSON file that can subsequently be used to create a Scheduled Job in Azure Batch.

Download the appropiate exeucutable for your OS in the Releases section.

```shell
root@94129eddf6a0:/source# ./az-batch-helper generate scheduled-job-config
Option '--output' is required.
Option '--scheduled-job-id' is required.
Option '--pool' is required.
Option '--schedule-recurrence' is required.
Option '--job-manager-image' is required.

Description:
  Generate a scheduled job json file for creating a scheduled job in Azure Batch.

Usage:
  az-batch-helper generate scheduled-job-config [options]

Options:
  --output <output> (REQUIRED)                             The output file for the generated artifact.
  --scheduled-job-id <scheduled-job-id> (REQUIRED)         The scheduled job id.
  --pool <pool> (REQUIRED)                                 The pool id.
  --schedule-recurrence <schedule-recurrence> (REQUIRED)   The schedule recurrence. ISO-8601 duration. (Example PT5M)
  --schedule-do-not-run-until <schedule-do-not-run-until>  The schedule do not run until. ISO-8601 date time. (Example 2030-08-07T11:43:00+00:00)
  --job-manager-command-line <job-manager-command-line>    The job manager command line. [default: ""]
  --job-manager-image <job-manager-image> (REQUIRED)       The job manager image.
  --job-manager-env-vars <job-manager-env-vars>            Comma seperated or newline seperated list of key=value environment variables.
  -?, -h, --help                                           Show help and usage information
```

# Example CLI Calls
## Windows
```powershell
.\az-batch-helper-win-x64.exe generate scheduled-job-config `
    --output output.json `
    --scheduled-job-id job-schedule-cicd `
    --pool my-pool `
    --schedule-recurrence PT5M `
    --schedule-do-not-run-until "2030-08-07T11:43:00+00:00" `
    --job-manager-image "myregistry.azurecr.io/job-manager-poc:1.0.13" `
    --job-manager-env-vars "KEY_VAULT_URL=https://my-key-vault.vault.azure.net,BATCH_ACCOUNT_KEY_SECRET_NAME=test-batch-account-key-secret-name,MANAGED_IDENTITY_RESOURCE_ID=/subscriptions/11111111-1111-1111-1111-11111111/resourceGroups/bicep-batch-test/providers/Microsoft.ManagedIdentity/userAssignedIdentities/test-managed-identity,IMAGE_NAME=job-execution-poc,IMAGE_TAG=1.0.0,IMAGE_REGISTRY=myregistry.azurecr.io"
```

Which generates this json:
```json
{
  "id": "job-schedule-cicd",
  "jobSpecification": {
    "jobManagerTask": {
      "commandLine": "\u0022\u0022",
      "containerSettings": {
        "imageName": "myregistry.azurecr.io/job-manager-poc:1.0.13"
      },
      "environmentSettings": [
        {
          "name": "KEY_VAULT_URL",
          "value": "https://my-key-vault.vault.azure.net"
        },
        {
          "name": "BATCH_ACCOUNT_KEY_SECRET_NAME",
          "value": "test-batch-account-key-secret-name"
        },
        {
          "name": "MANAGED_IDENTITY_RESOURCE_ID",
          "value": "/subscriptions/11111111-1111-1111-1111-11111111/resourceGroups/bicep-batch-test/providers/Microsoft.ManagedIdentity/userAssignedIdentities/test-managed-identity"
        },
        {
          "name": "IMAGE_NAME",
          "value": "job-execution-poc"
        },
        {
          "name": "IMAGE_TAG",
          "value": "1.0.0"
        },
        {
          "name": "IMAGE_REGISTRY",
          "value": "myregistry.azurecr.io"
        }
      ],
      "id": "job-manager-task",
      "killJobOnCompletion": false
    },
    "onAllTasksComplete": "terminatejob",
    "poolInfo": {
      "autoPoolSpecification": null,
      "poolId": "my-pool"
    }
  },
  "schedule": {
    "recurrenceInterval": "PT5M",
    "doNotRunUntil": "2030-08-07T11:43:00\u002B00:00"
  }
}
```

and can be submitted to Azure Batch via 
```powershell
az batch job-schedule create `
     --account-name AZURE_BATCH_ACCOUNT_NAME `
     --account-endpoint AZURE_BATCH_ACCOUNT_NAME.LOCATION.batch.azure.com `
     --json-file .\output.json
```
## Linux
```shell
./az-batch-helper-linux-x64 generate scheduled-job-config \
    --output output.json \
    --scheduled-job-id job-schedule-cicd \
    --pool my-pool \
    --schedule-recurrence PT5M \
    --schedule-do-not-run-until "2030-08-07T11:43:00+00:00" \
    --job-manager-image "myregistry.azurecr.io/job-manager-poc:1.0.13" \
    --job-manager-env-vars "KEY_VAULT_URL=https://my-key-vault.vault.azure.net,BATCH_ACCOUNT_KEY_SECRET_NAME=test-batch-account-key-secret-name,MANAGED_IDENTITY_RESOURCE_ID=/subscriptions/11111111-1111-1111-1111-11111111/resourceGroups/bicep-batch-test/providers/Microsoft.ManagedIdentity/userAssignedIdentities/test-managed-identity,IMAGE_NAME=job-execution-poc,IMAGE_TAG=1.0.0,IMAGE_REGISTRY=myregistry.azurecr.io"
```
# Running in a GHA Workflow
```shell
- name: Download az-batch-helper CLI
  run: |
      curl -LO https://github.com/hotfix-houdini/cli-az-batch-helper/releases/download/v1.6/az-batch-helper-linux-x64
- name: Make CLI executable
  run: chmod +x az-batch-helper-linux-x64
- name: Create Scheduled job JSON file
  run: |
    ./az-batch-helper-linux-x64 generate scheduled-job-config \
      --output scheduled-job.json \
      --scheduled-job-id job-schedule-cicd \
      --pool my-pool \
      --schedule-recurrence PT5M \
      --schedule-do-not-run-until "2030-08-07T11:43:00+00:00" \
      --job-manager-image "myregistry.azurecr.io/job-manager-poc:1.0.13" \
      --job-manager-env-vars "\
        KEY_VAULT_URL=https://my-key-vault.vault.azure.net,\
        BATCH_ACCOUNT_KEY_SECRET_NAME=test-batch-account-key-secret-name,\
        MANAGED_IDENTITY_RESOURCE_ID=/subscriptions/11111111-1111-1111-1111-11111111/resourceGroups/bicep-batch-test/providers/Microsoft.ManagedIdentity/userAssignedIdentities/test-managed-identity,\
        IMAGE_NAME=job-execution-poc,\
        IMAGE_TAG=1.0.0,\
        IMAGE_REGISTRY=myregistry.azurecr.io"
- name: Create schedule job 
  run: |
    az batch job-schedule create \
      --account-name "$BATCH_ACCOUNT_NAME" \
      --account-endpoint "$BATCH_ACCOUNT_ENDPOINT" \
      --json-file scheduled-job.json
```
