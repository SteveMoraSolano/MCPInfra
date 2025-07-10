using Azure;
using Azure.ResourceManager;
using Azure.ResourceManager.Storage;
using System;
using System.Collections.Generic;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Threading.Tasks;
using MCPServer.Tools;

[McpServerToolType]

public static class StorageTools
{

    [McpServerTool, Description("List Storage Account By Resource Group Name")]
    public static async Task<List<string>> funListStorageAccountByResourceGroup(string pSubscriptionId, string pResourceGroupName)
    {
        var vResult = new List<string>();
        try
        {
            var vArmClient = AZ_AuthHelper.funGetArmClient(pSubscriptionId);
            var vSubscription = await vArmClient.GetDefaultSubscriptionAsync();
            var vResourceGroup = vSubscription.GetResourceGroup(pResourceGroupName).Value;
            var vStorageAccount = vResourceGroup.GetStorageAccounts();

            await foreach (var vSA in vStorageAccount)
            {
                vResult.Add($"{vSA.Data.Name} ({vSA.Data.Id})");
            }

        }
        catch (Exception Ex)
        {
            vResult.Add($"[ERROR] {Ex.Message}");
        }

        return vResult;
    }
     [McpServerTool, Description("List All Storage Account")]
    public static async Task<List<string>> funListAllStorageAccounts(string pSubscriptionId)
    {
        var vResult = new List<string>();
        try
        {
            var vArmClient = AZ_AuthHelper.funGetArmClient(pSubscriptionId);
            var vSubscription = await vArmClient.GetDefaultSubscriptionAsync();
            var vResourceGroups = vSubscription.GetResourceGroups();

            await foreach (var vRg in vResourceGroups)
            {
                var vStorageAccounts = vRg.GetStorageAccounts();
                await foreach (var vSA in vStorageAccounts)
                {
                    vResult.Add($"{vSA.Data.Name} ({vSA.Data.Id})");
                }
            }
        }
        catch (Exception Ex)
        {
            vResult.Add($"[ERROR] {Ex.Message}");
        }

        return vResult;
    }

}

