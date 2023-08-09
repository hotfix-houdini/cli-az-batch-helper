[![ci-dotnet](https://github.com/hotfix-houdini/cli-az-batch-helper/actions/workflows/ci-dotnet.yml/badge.svg)](https://github.com/hotfix-houdini/cli-az-batch-helper/actions/workflows/ci-dotnet.yml)

# cli-az-batch-helper
## Example CLI call (windows)
```powershell
.\AzBatchHelper.Cli.exe generate scheduled-job-config `
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
