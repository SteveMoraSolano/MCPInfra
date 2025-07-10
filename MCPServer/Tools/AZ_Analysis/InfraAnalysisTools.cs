using Azure.ResourceManager;
using Azure.ResourceManager.Compute;
using Azure.ResourceManager.Network;
using Azure.ResourceManager.Compute.Models;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Diagnostics;
using ModelContextProtocol.Server;

namespace MCPServer.Tools;

[McpServerToolType]
public static class InfraAnalysisTools
{
[McpServerTool, Description("Analyze a resource group infrastructure and return metadata for best practice evaluation. Valid resource types: vm, vnet.")]
public static async Task<List<string>> funAnalyzeInfrastructure(string pSubscriptionId, string pResourceGroupName)
{
    var vResult = new List<string>();

    try
    {
        var vArmClient = AZ_AuthHelper.funGetArmClient(pSubscriptionId);
        var vSubscription = await vArmClient.GetDefaultSubscriptionAsync();
        var vResourceGroup = vSubscription.GetResourceGroup(pResourceGroupName).Value;

        // Get and summarize Virtual Machines
        var vVmList = vResourceGroup.GetVirtualMachines();
        await foreach (var vVm in vVmList)
        {
            vResult.Add($"🔍 VirtualMachine: {vVm.Data.Name}");
            vResult.Add($"- VM Size: {vVm.Data.HardwareProfile?.VmSize}");
            vResult.Add($"- OS Type: {vVm.Data.StorageProfile?.OSDisk?.OSType}");
            vResult.Add($"- Admin Username: {vVm.Data}");
            vResult.Add($"- Location: {vVm.Data.Location}");
            vResult.Add($"- Public IP Assigned: {(vVm.Data.NetworkProfile?.NetworkInterfaces.Any() == true ? "Yes" : "No")}");
        }

        // Get and summarize Virtual Networks
        var vVNetList = vResourceGroup.GetVirtualNetworks();
        await foreach (var vVNet in vVNetList)
        {
            vResult.Add($"\n🔍 VirtualNetwork: {vVNet.Data.Name}");
            foreach (var vPrefix in vVNet.Data.AddressPrefixes)
            {
                vResult.Add($"- Address Prefix: {vPrefix}");
            }
            foreach (var vSubnet in vVNet.Data.Subnets)
            {
                vResult.Add($"- Subnet: {vSubnet.Name} ({string.Join(",", vSubnet.AddressPrefix)})");
            }
        }

        // Add best practices for VM
        var vVmBps = await KnowledgeBaseTools.funGetBestPracticesByResourceType("vm");
        vResult.Add("\n📘 Best Practices (VM):");
        vResult.AddRange(vVmBps);

        // Add best practices for VNet
        var vVnetBps = await KnowledgeBaseTools.funGetBestPracticesByResourceType("vnet");
        vResult.Add("\n📘 Best Practices (VNet):");
        vResult.AddRange(vVnetBps);

        // Ready for AI Analysis
        vResult.Add($"\n✅ Infrastructure data collected successfully. Ready for best-practice evaluation.");
    }
    catch (Exception ex)
    {
        vResult.Add($"[ERROR] {ex.Message}");
    }

    return vResult;
}

}
