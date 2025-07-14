using Azure.ResourceManager.ContainerInstance;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MCPServer.Tools;

public static class ContainerAnalyzerTools
{
   [McpServerTool, Description("Analyze Azure Container Instances (ACI) in a Resource Group.")]
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
            vResult.Add($"- IP Type: {vCG_Data.IPAddress?.GetType()}");
            vResult.Add($"- DNS Name Label: {vCG_Data.IPAddress?.DnsNameLabel}");
            vResult.Add($"- Public IP: {vCG_Data.IPAddress?.IP}");

            foreach (var vInstance in vCG_Data.Containers)
            {
                vResult.Add($"  Container: {vInstance.Name}");
                vResult.Add($"  - Image: {vInstance.Image}");
                vResult.Add($"  - CPU: {vInstance.Resources.Requests?.Cpu}");
                vResult.Add($"  - Memory: {vInstance.Resources.Requests?.MemoryInGB} GB");
                vResult.Add($"  - Ports: {string.Join(",", vInstance.Ports.Select(p => p.Port))}");
            }
        }

        var vContainerBps = await KnowledgeBaseTools.funGetBestPracticesByResourceType("containers");
        vResult.Add("\n📘 Best Practices (Containers):");
        vResult.AddRange(vContainerBps);
    }
    catch (Exception ex)
    {
        vResult.Add($"[ERROR] {ex.Message}");
    }

    return vResult;
}

}
