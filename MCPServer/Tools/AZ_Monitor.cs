using Azure;
using Azure.ResourceManager;
using Azure.ResourceManager.OperationalInsights;
using System;
using System.Collections.Generic;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Threading.Tasks;
using MCPServer.Tools;

[McpServerToolType]

public static class MonitorTools
{

    [McpServerTool, Description("List Log Analytics Workspace By Resource Group Name")]
    public static async Task<List<string>> funListLogAnalyticsWorkspaceByResourceGroup(string pSubscriptionId, string pResourceGroupName)
    {
        var vResult = new List<string>();
        try
        {
            var vArmClient = AZ_AuthHelper.funGetArmClient(pSubscriptionId);
            var vSubscription = await vArmClient.GetDefaultSubscriptionAsync();
            var vResourceGroup = vSubscription.GetResourceGroup(pResourceGroupName).Value;
            var vWorkspace = vResourceGroup.GetOperationalInsightsWorkspaces();

            await foreach (var vWS in vWorkspace)
            {
                vResult.Add($"Name: {vWS.Data.Name} | Location: {vWS.Data.Location} | SKU: {vWS.Data.Sku?.Name} | Retention: {vWS.Data.RetentionInDays} days | ID: {vWS.Data.Id}");
            }

        }
        catch (Exception Ex)
        {
            vResult.Add($"[ERROR] {Ex.Message}");
        }

        return vResult;
    }
     [McpServerTool, Description("List All Log Analytics Workspace")]
    public static async Task<List<string>> funListLogAnalyticsWorkspace(string pSubscriptionId)
    {
        var vResult = new List<string>();
        try
        {
            var vArmClient = AZ_AuthHelper.funGetArmClient(pSubscriptionId);
            var vSubscription = await vArmClient.GetDefaultSubscriptionAsync();
            var vResourceGroups = vSubscription.GetResourceGroups();

            await foreach (var vRg in vResourceGroups)
            {
                var vWorkspaces = vRg.GetOperationalInsightsWorkspaces();
                await foreach (var vWS in vWorkspaces)
                {
                    vResult.Add($"RG: {vRg.Data.Name} | Name: {vWS.Data.Name} | Location: {vWS.Data.Location} | SKU: {vWS.Data.Sku?.Name} | Retention: {vWS.Data.RetentionInDays} days | ID: {vWS.Data.Id}");
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

