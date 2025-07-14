using Azure.ResourceManager.AppService;
using Azure.ResourceManager.AppService.Models;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MCPServer.Tools;

public static class AppServiceAnalyzerTools
{
   [McpServerTool, Description("Analyze Azure App Services (Web Apps) in a Resource Group.")]
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

            vResult.Add($"\n🔍 App Service: {vApp_Data.Name}");
            vResult.Add($"- Location: {vApp_Data.Location}");
            vResult.Add($"- State: {vApp_Data.State}");
            vResult.Add($"- Hosting Plan: {vApp_Data.AppServicePlanId}");
            vResult.Add($"- HTTPS Only: {vApp_Data.IsHttpsOnly}");
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
            var vSettings = vAppSettings.Value.Properties;

            var hasLogging = vSettings.ContainsKey("WEBSITE_HTTPLOGGING_ENABLED");
            vResult.Add($"- HTTP Logging Enabled: {(hasLogging ? "Yes" : "No")}");

            // VNet Integration
            var vVNetRouteAllEnabled = vAppSettings.Value.Properties.TryGetValue("WEBSITE_VNET_ROUTE_ALL", out var vNetValue) && vNetValue == "1";
            vResult.Add($"- VNet Route All: {(vVNetRouteAllEnabled ? "Enabled" : "Disabled")}");
        }

        var vAppBp = await KnowledgeBaseTools.funGetBestPracticesByResourceType("appservices");
        vResult.Add("\n📘 Best Practices (App Services):");
        vResult.AddRange(vAppBp);
    }
    catch (Exception ex)
    {
        vResult.Add($"[ERROR] {ex.Message}");
    }

    return vResult;
}

}
