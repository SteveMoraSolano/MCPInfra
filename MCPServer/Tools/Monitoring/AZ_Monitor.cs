using Azure;
using Azure.ResourceManager;
using Azure.ResourceManager.OperationalInsights;
using Azure.ResourceManager.Monitor;
using Azure.ResourceManager.Monitor.Models;
using Azure.Monitor;
using Azure.Monitor.Query;
using Azure.Monitor.Query.Models;
using System;
using System.Collections.Generic;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Threading.Tasks;
using MCPServer.Tools;
using Azure.Core;


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

    [McpServerTool, Description("Query Log Analytics using Kusto Query Language (KQL)")]
    public static async Task<List<string>> funQueryLogAnalytics(string pWorkspaceId, string pQuery)
    {
        var vResult = new List<string>();
        try
        {
            var credential = AZ_AuthHelper.funGetTokenCredential();
            var client = new LogsQueryClient(credential);

            var response = await client.QueryWorkspaceAsync(pWorkspaceId, pQuery, TimeSpan.FromHours(1));
            foreach (var row in response.Value.Table.Rows)
            {
                vResult.Add(string.Join(" | ", row));
            }
        }
        catch (Exception ex)
        {
            vResult.Add($"[ERROR] {ex.Message}");
        }

        return vResult;
    }


    
[McpServerTool, Description("Configure diagnostic settings to send logs to a Log Analytics Workspace")]
public static async Task<string> funConfigureDiagnosticSettings(string pSubscriptionId, string pResourceId, string pWorkspaceId)
{
    try
    {
        var vArmClient = AZ_AuthHelper.funGetArmClient(pSubscriptionId);
        var vResourceIdentifier = new ResourceIdentifier(pResourceId);
        var vDiagnosticSettingsCollection = vArmClient.GetDiagnosticSettings(vResourceIdentifier);
        var vData = new DiagnosticSettingData
        {
            WorkspaceId = new ResourceIdentifier(pWorkspaceId)
        };

        vData.Logs.Add(new LogSettings(true)
        {
            RetentionPolicy = new RetentionPolicy(true, 0)
        });

        vData.Metrics.Add(new MetricSettings(true) 
        {
            RetentionPolicy = new RetentionPolicy(true, 0)
        });

        var vOperation = await vDiagnosticSettingsCollection.CreateOrUpdateAsync(
            WaitUntil.Completed, 
            "mcp-diagnostics", 
            vData
        );
        
        return $"Diagnostic settings configured successfully for resource: {pResourceId}";
    }
    catch (Exception ex)
    {
        return $"[ERROR] Failed to configure diagnostic settings: {ex.Message}";
    }
}
   



}

