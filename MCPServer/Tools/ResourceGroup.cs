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

    public static async Task<List<string>> funListResourceGroups(
        string pSubscriptionId)

    {
        var vResult = new List<string>();

        try
        {

            var vArmClient = AZ_AuthHelper.funGetArmClient(pSubscriptionId);

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

         string pResourceGroupName,string pSubscriptionId)

    {
        var vResult = new List<string>();

        try
        {

            var vArmClient = AZ_AuthHelper.funGetArmClient(pSubscriptionId);

            var vSubscription = await vArmClient.GetDefaultSubscriptionAsync();
            var vResourceGroup = await vSubscription.GetResourceGroups().GetAsync(pResourceGroupName);

            foreach (var vGenericResource in vResourceGroup.Value.GetGenericResources())
            {
                vResult.Add($"- {vGenericResource.Data.ResourceType} => {vGenericResource.Data.Name}");
            }

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

       string pResourceGroupName,string pLocation,string pSubscriptionId)
    {
        try
        {
            var vArmClient = AZ_AuthHelper.funGetArmClient(pSubscriptionId);

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
