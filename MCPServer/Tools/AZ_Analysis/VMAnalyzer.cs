using Azure.ResourceManager.Compute;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MCPServer.Tools;

public static class VMAnalyzerTools
{
    [McpServerTool, Description("Analyze Virtual Machines in a resource group and return VM metadata.")]
    public static async Task<List<string>> funAnalyzeVMs(string pSubscriptionId, string pResourceGroupName)
    {
        var vResult = new List<string>();
        try
        {
            var vArmClient = AZ_AuthHelper.funGetArmClient(pSubscriptionId);
            var vSubscription = await vArmClient.GetDefaultSubscriptionAsync();
            var vResourceGroup = vSubscription.GetResourceGroup(pResourceGroupName).Value;
            var vVmList = vResourceGroup.GetVirtualMachines();

            await foreach (var vVm in vVmList)
            {
                var vVmData = vVm.Data;
                vResult.Add($"VirtualMachine: {vVmData.Name}");
                vResult.Add($"- VM Size: {vVmData.HardwareProfile?.VmSize}");
                vResult.Add($"- OS Type: {vVmData.StorageProfile?.OSDisk?.OSType}");
                vResult.Add($"- Location: {vVmData.Location}");
                vResult.Add($"- Public IP Assigned: {(vVmData.NetworkProfile?.NetworkInterfaces.Any() == true ? "Yes" : "No")}");
            }

            var vBestPractices = await KnowledgeBaseTools.funGetBestPracticesByResourceType("vm");
            vResult.Add("\n📘 Best Practices (VM):");
            vResult.AddRange(vBestPractices);
        }
        catch (Exception ex)
        {
            vResult.Add($"[ERROR] VM Analysis Failed: {ex.Message}");
        }

        return vResult;
    }
}
