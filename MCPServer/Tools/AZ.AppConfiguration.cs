using Azure;
using Azure.ResourceManager;
using Azure.ResourceManager.AppConfiguration;
using System;
using System.Collections.Generic;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Threading.Tasks;
using MCPServer.Tools;

[McpServerToolType]

public static class AppConfigurationTools
{

    [McpServerTool, Description("List App Configuration Stores By Resource Group Name")]
    public static async Task<List<string>> funListAppConfigurationStoresByResourceGroup(string pSubscriptionId, string pResourceGroupName)
    {
        var vResult = new List<string>();
        try
        {
            var vArmClient = AZ_AuthHelper.funGetArmClient(pSubscriptionId);
            var vSubscription = await vArmClient.GetDefaultSubscriptionAsync();
            var vResourceGroup = vSubscription.GetResourceGroup(pResourceGroupName).Value;
            var vConfigStore = vResourceGroup.GetAppConfigurationStores();

            await foreach (var vCS in vConfigStore)
            {
                vResult.Add($"{vCS.Data.Name} ({vCS.Data.Id})");
            }

        }
        catch (Exception Ex)
        {
            vResult.Add($"[ERROR] {Ex.Message}");
        }

        return vResult;
    }
     [McpServerTool, Description("List All App Configuration Stores")]
    public static async Task<List<string>> funListAppConfigurationStores(string pSubscriptionId)
    {
        var vResult = new List<string>();
        try
        {
            var vArmClient = AZ_AuthHelper.funGetArmClient(pSubscriptionId);
            var vSubscription = await vArmClient.GetDefaultSubscriptionAsync();
            var vResourceGroups = vSubscription.GetResourceGroups();

            await foreach (var vRg in vResourceGroups)
            {
                var vConfigStore = vRg.GetAppConfigurationStores();
                await foreach (var vCS in vConfigStore)
                {
                    vResult.Add($"{vCS.Data.Name} ({vCS.Data.Id})");
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

