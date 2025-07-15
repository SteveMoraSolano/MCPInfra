using Azure.ResourceManager.ContainerInstance;
using Azure.ResourceManager.ContainerInstance.Models;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MCPServer.Tools;

[McpServerToolType]
public static class ContainerAnalyzerTools
{
   [McpServerTool, Description("Analyze Azure Container Instances (ACI) in a Resource Group with deep insights and best-practice evaluation.")]
public static async Task<List<string>> funAnalyzeContainerInstances(string pSubscriptionId, string pResourceGroupName)
{
    var vResult = new List<string>();

        try
        {
            var vArmClient = AZ_AuthHelper.funGetArmClient(pSubscriptionId);
            var vSubscription = await vArmClient.GetDefaultSubscriptionAsync();
            var vResourceGroup = vSubscription.GetResourceGroup(pResourceGroupName).Value;
            var vContainerGroups = vResourceGroup.GetContainerGroups();

            await foreach (var vCG in vContainerGroups)
            {
                var vCG_Data = vCG.Data;

                vResult.Add($"\nContainer Group: {vCG_Data.Name}");
                vResult.Add($"- Location: {vCG_Data.Location}");
                vResult.Add($"- OS Type: {vCG_Data.OSType}");
                vResult.Add($"- Restart Policy: {vCG_Data.RestartPolicy}");
                vResult.Add($"- Provisioning State: {vCG_Data.ProvisioningState}");
                vResult.Add($"- IP Address Type: {vCG_Data.IPAddress?.GetType()}");
                vResult.Add($"- DNS Name Label: {vCG_Data.IPAddress?.DnsNameLabel ?? "None"}");
                vResult.Add($"- Public IP: {(vCG_Data.IPAddress != null ? vCG_Data.IPAddress.IP : "None")}");

                // Networking
                if (vCG_Data.SubnetIds != null && vCG_Data.SubnetIds.Count > 0)
                    vResult.Add("VNet Integration: Configured");
                else
                    vResult.Add("VNet Integration: Not Configured");

                // Diagnostics
                if (vCG_Data.DiagnosticsLogAnalytics != null)
                {
                    vResult.Add("Log Analytics: Enabled");
                    vResult.Add($"  - Workspace: {vCG_Data.DiagnosticsLogAnalytics.WorkspaceId}");
                }
                else
                {
                    vResult.Add("Log Analytics: Not Configured");
                }

                // Containers
                foreach (var vInstance in vCG_Data.Containers)
                {
                    vResult.Add($"  Container: {vInstance.Name}");
                    vResult.Add($"    - Image: {vInstance.Image}");
                    vResult.Add($"    - CPU Request: {vInstance.Resources?.Requests?.Cpu}");
                    vResult.Add($"    - Memory Request: {vInstance.Resources?.Requests?.MemoryInGB} GB");
                    vResult.Add($"    - Ports: {string.Join(", ", vInstance.Ports.Select(p => p.Port))}");

                    // Environment Variables
                    if (vInstance.EnvironmentVariables.Any(e => !string.IsNullOrEmpty(e.SecureValue)))
                        vResult.Add("    - Secure Env Vars: Present");
                    else
                        vResult.Add("    - Secure Env Vars: Not Configured");

                    if (vInstance.Command.Count > 0)
                        vResult.Add($"    - Command: {string.Join(" ", vInstance.Command)}");
                }

                // Tags
                if (vCG_Data.Tags != null && vCG_Data.Tags.Count > 0)
                    vResult.Add("Tags: Present");
                else
                    vResult.Add("Tags: Missing");
            }


            var vContainerBps = await KnowledgeBaseTools.funGetBestPracticesByResourceType("containers");
            vResult.Add("\nBest Practices (Containers):");
            vResult.AddRange(vContainerBps);
        
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
