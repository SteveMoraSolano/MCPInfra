using Azure;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using ModelContextProtocol.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using MCPServer.Tools;


[McpServerToolType]
public static class ResourceGroupTools
{
    [McpServerTool, Description("Lists all Azure Resource Groups in the subscription.")]
    public static async Task<List<string>> funListResourceGroups()
    {
        var vResult = new List<string>();

        try
        {
            var vArmClient = AZ_AuthHelper.funGetArmClient();
            var vSubscription = await vArmClient.GetDefaultSubscriptionAsync();
            var vResourceGroups = vSubscription.GetResourceGroups();

            await foreach (var vRg in vResourceGroups.GetAllAsync())
            {
                vResult.Add(vRg.Data.Name);
            }

            return vResult;
        }
        catch (Exception ex)
        {
            vResult.Add($"[ERROR] Could not retrieve resource groups: {ex.Message}");
            return vResult;
        }
    }
[McpServerTool, Description("Lists all resources inside a given Azure Resource Group.")]
    public static async Task<List<string>> funListResourcesInResourceGroup(
        [Description("Name of the Resource Group.")] string pResourceGroupName)
    {
        var vResult = new List<string>();

        try
        {
            var vArmClient = AZ_AuthHelper.funGetArmClient();
            var vSubscription = await vArmClient.GetDefaultSubscriptionAsync();
            var vResourceGroup = await vSubscription.GetResourceGroups().GetAsync(pResourceGroupName);

            foreach (var vGenericResource in vResourceGroup.Value.GetGenericResources())
            {
                vResult.Add($"- {vGenericResource.Data.ResourceType} => {vGenericResource.Data.Name}");
            }

            if (vResult.Count == 0)
                vResult.Add($"[INFO] No resources found in resource group '{pResourceGroupName}'.");

            return vResult;
        }
        catch (Exception ex)
        {
            vResult.Add($"[ERROR] Could not list resources: {ex.Message}");
            return vResult;
        }
    }
    [McpServerTool, Description("Creates a new Azure Resource Group.")]
    public static async Task<string> funCreateResourceGroup(
        [Description("Name of the Resource Group.")] string pResourceGroupName,
        [Description("Azure region (e.g., eastus, westeurope).")] string pLocation)
    {
        try
        {
            var vArmClient = AZ_AuthHelper.funGetArmClient();
            var vSubscription = await vArmClient.GetDefaultSubscriptionAsync();
            var vRgData = new ResourceGroupData(pLocation);

            var vRgLro = await vSubscription.GetResourceGroups()
                                            .CreateOrUpdateAsync(WaitUntil.Completed, pResourceGroupName, vRgData);

            var vRg = vRgLro.Value;

            return $"[SUCCESS] Resource Group '{vRg.Data.Name}' created in '{vRg.Data.Location}'.";
        }
        catch (Exception ex)
        {
            return $"[ERROR] Failed to create Resource Group: {ex.Message}";
        }
    }
}
