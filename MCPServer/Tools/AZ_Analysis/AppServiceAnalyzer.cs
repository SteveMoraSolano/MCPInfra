using Azure.ResourceManager.AppService;
using Azure.ResourceManager.AppService.Models;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MCPServer.Tools;

[McpServerToolType]
public static class AppServiceAnalyzerTools
{
   [McpServerTool, Description("Analyze Azure App Services (Web Apps) in a Resource Group with deep insights and best-practice evaluation.")]
public static async Task<List<string>> funAnalyzeAppServices(string pSubscriptionId, string pResourceGroupName)
{
    var vResult = new List<string>();

        try
        {
            var vArmClient = AZ_AuthHelper.funGetArmClient(pSubscriptionId);
            var vSubscription = await vArmClient.GetDefaultSubscriptionAsync();
            var vResourceGroup = vSubscription.GetResourceGroup(pResourceGroupName).Value;
            var vWebApps = vResourceGroup.GetWebSites();

            await foreach (var vApp in vWebApps)
            {
                var vApp_Data = vApp.Data;

                vResult.Add($"\nApp Service: {vApp_Data.Name}");
                vResult.Add($"- Location: {vApp_Data.Location}");
                vResult.Add($"- State: {vApp_Data.State}");
                vResult.Add($"- App Service Plan: {vApp_Data.AppServicePlanId}");
                vResult.Add($"- HTTPS Only: {vApp_Data.IsHttpsOnly}");
                vResult.Add($"- Always On: {vApp_Data.SiteConfig?.IsAlwaysOn}");
                vResult.Add($"- Enabled: {vApp_Data.IsEnabled}");


                // Slots
                var vSlotCount = 0;
                try
                {
                    var vSlots = vApp.GetWebSiteSlots();
                    await foreach (var _ in vSlots)
                    {
                        vSlotCount++;
                    }
                    vResult.Add($"- Deployment Slots: {vSlotCount}");
                }
                catch
                {
                    vResult.Add($"- Deployment Slots: Unable to retrieve");
                }

                // Auth
                vResult.Add($"- Client Affinity Enabled: {vApp_Data.IsClientAffinityEnabled}");
                vResult.Add($"- Client Cert Enabled: {vApp_Data.IsClientCertEnabled}");

                // App Settings
                var vAppSettings = await vApp.GetApplicationSettingsAsync();
                var settings = vAppSettings.Value.Properties;
                vResult.Add($"- HTTP Logging: {(settings.ContainsKey("WEBSITE_HTTPLOGGING_ENABLED") ? "Enabled" : "Disabled")}");
                vResult.Add($"- Diagnostic Logging to Blob: {(settings.ContainsKey("DIAGNOSTICS_AZUREBLOBCONTAINERSASURL") ? "Configured" : "Not Configured")}");
                vResult.Add($"- VNet Route All: {(settings.TryGetValue("WEBSITE_VNET_ROUTE_ALL", out var vNetRoute) && vNetRoute == "1" ? "Enabled" : "Disabled")}");

                // Backup
                vResult.Add($"- Backup Configured: {(vApp_Data.Tags != null && vApp_Data.Tags.ContainsKey("backup") ? "Yes" : "Unknown")}");

                // TLS Version
                vResult.Add($"- Minimum TLS Version: {vApp_Data.SiteConfig?.MinTlsVersion}");

                // Tags
                if (vApp_Data.Tags != null && vApp_Data.Tags.Count > 0)
                    vResult.Add("Tags: Present");
                else
                    vResult.Add("Tags: Missing (owner, env, cost center)");
            }


            var vAppBp = await KnowledgeBaseTools.funGetBestPracticesByResourceType("appservices");
            vResult.Add("\nBest Practices (App Services):");
            vResult.AddRange(vAppBp);
        
              var vSecurityBp = await KnowledgeBaseTools.funGetBestPracticesByResourceType("security");
            vResult.Add("\nBest Practices (Security):");
            vResult.AddRange(vSecurityBp);

            var vCostBp = await KnowledgeBaseTools.funGetBestPracticesByResourceType("cost-optimization");
            vResult.Add("\nBest Practices (Cost Optimization):");
            vResult.AddRange(vCostBp);

            var vPerfBp = await KnowledgeBaseTools.funGetBestPracticesByResourceType("performance");
            vResult.Add("\nBest Practices (Performance):");
            vResult.AddRange(vPerfBp);

            var vGovBp = await KnowledgeBaseTools.funGetBestPracticesByResourceType("governance");
            vResult.Add("\nBest Practices (Governance):");
            vResult.AddRange(vGovBp);

            var vMonitoringBp = await KnowledgeBaseTools.funGetBestPracticesByResourceType("monitoring");
            vResult.Add("\nBest Practices (Monitoring):");
            vResult.AddRange(vMonitoringBp);
    }
        catch (Exception ex)
        {
            vResult.Add($"[ERROR] {ex.Message}");
        }

    return vResult;
}

}
